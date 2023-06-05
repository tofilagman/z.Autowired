using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace z.Autowire.Attributes
{
    /// <summary>
    /// Specifies that a parameter or property should be bound using a configuration setting coming from the
    /// <see cref="IConfiguration"/> service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValueAttribute : Attribute, IDependencyResolver
    {
        private static readonly MethodInfo _asReadOnlyMethodInfo = typeof(Array).GetMethod(nameof(Array.AsReadOnly))!;

        public ValueAttribute(string configurationKey)
        {
            ConfigurationKey = configurationKey;
        }

        /// <summary>
        /// Gets or sets the configuration key to use.
        /// </summary>
        public string ConfigurationKey { get; set; }

        public object? Resolve(IServiceProvider serviceProvider, Type type)
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            string value = configuration[ConfigurationKey];

            if (type.IsGenericType)
            {
                if (ImplementsGenericType(type, typeof(IReadOnlyList<>), out Type typeParameter))
                    return CreateTypedReadOnlyList(serviceProvider, typeParameter, configuration);
                else if (ImplementsGenericType(type, typeof(IList<>), out typeParameter))
                    return CreateTypedArray(serviceProvider, typeParameter, configuration);
            }

            return ResolveValue(type, value, serviceProvider);
        }

        private static object? ResolveValue(Type type, string value, IServiceProvider serviceProvider)
        {
            if (type == typeof(string))
                return value;

            Type? nullableOf = Nullable.GetUnderlyingType(type);

            if (nullableOf != null)
                return value == null ? null : ResolveValue(nullableOf, value, serviceProvider);
            else if (type == typeof(TimeSpan))
                return TimeSpan.Parse(value);
            else if (type == typeof(Guid))
                return Guid.Parse(value);
            else if (type == typeof(Uri))
                return new Uri(value);
            else if (type.IsEnum)
                return Enum.Parse(type, value);
            else
            {
                try
                {
                    return Convert.ChangeType(value, type);
                }
                catch (InvalidCastException)
                {
                    MethodInfo? parse = type.GetMethod(
                        "Parse",
                        BindingFlags.Static | BindingFlags.Public,
                        null,
                        new[] { typeof(string) },
                        Array.Empty<ParameterModifier>());

                    if (parse != null && !parse.IsGenericMethod)
                        return parse.Invoke(null, new object[] { value });
                    else
                        throw;
                }
            }
        }

        private static bool ImplementsGenericType(Type type, Type openType, out Type typeParameter)
        {
            foreach (Type implementedInterface in openType.GetInterfaces().Append(openType))
            {
                if (implementedInterface.IsGenericType &&
                    type.GetGenericTypeDefinition() == implementedInterface.GetGenericTypeDefinition())
                {
                    typeParameter = type.GenericTypeArguments[0];
                    return true;
                }
            }

            typeParameter = null!;
            return false;
        }

        private object CreateTypedArray(IServiceProvider serviceProvider, Type type, IConfiguration configuration)
        {
            List<string> children = configuration
                .GetSection(ConfigurationKey)
                .GetChildren()
                .Select(child => child.Value)
                .ToList();

            Array result = Array.CreateInstance(type, children.Count);

            for (int i = 0; i < result.Length; i++)
                result.SetValue(ResolveValue(type, children[i], serviceProvider), i);

            return result;
        }

        private object? CreateTypedReadOnlyList(IServiceProvider serviceProvider, Type type, IConfiguration configuration)
        {
            object array = CreateTypedArray(serviceProvider, type, configuration);

            return _asReadOnlyMethodInfo.MakeGenericMethod(type).Invoke(null, new[] { array });
        }
    }
}
