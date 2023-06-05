using Microsoft.Extensions.DependencyInjection;

namespace z.Autowire.Attributes
{
    /// <summary>
    /// Specifies that a parameter or property should be bound using dependency injection resolution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AutowiredAttribute : Attribute, IDependencyResolver
    {
        /// <summary>
        /// Gets or sets a boolean value indicating whether the service is required or optional.
        /// </summary>
        public bool Optional { get; set; }

        public object? Resolve(IServiceProvider serviceProvider, Type type)
        {
            if (Optional)
                return serviceProvider.GetService(type);
            else
                return serviceProvider.GetRequiredService(type);
        }
    }
}
