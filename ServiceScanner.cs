using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using z.Autowire.Attributes;

namespace z.Autowire
{
    public static class ServiceScanner
    {
        public static IEnumerable<Func<ServiceDescriptor>> ScanServiceRegistrations(Type type, IServiceProvider serviceProvider)
        {
            IServiceActivator serviceActivator = serviceProvider.GetRequiredService<IServiceActivator>();

            if (!type.IsAbstract && CanScan(type, serviceProvider))
            {
                foreach (ServiceAttribute registerAttribute in type.GetCustomAttributes<ServiceAttribute>())
                {
                    Type serviceType = registerAttribute.ServiceType ?? type;

                    if (!serviceType.IsAssignableFrom(type))
                    {
                        throw new ArgumentException(
                            $"The concrete type '{type.FullName}' cannot be used to register service type '{serviceType.FullName}'.");
                    }

                    yield return () => new ServiceDescriptor(
                        serviceType,
                        serviceActivator.GetFactory(type),
                        registerAttribute.Scope);
                }
            }
        }

        public static IEnumerable<Func<ServiceDescriptor>> ScanFactoryRegistrations(Type type, IServiceProvider serviceProvider)
        {
            if (CanScan(type, serviceProvider))
            {
                IServiceActivator serviceActivator = serviceProvider.GetRequiredService<IServiceActivator>();

                foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    if (CanScan(method, serviceProvider))
                    {
                        foreach (BeanAttribute registerAttribute in method.GetCustomAttributes<BeanAttribute>())
                        {
                            Type serviceType = registerAttribute.ServiceType ?? method.ReturnType;

                            if (!serviceType.IsAssignableFrom(method.ReturnType))
                            {
                                throw new ArgumentException(
                                    $"The method '{method.Name}' with return type '{method.ReturnType}' cannot be used " +
                                    $"to register service type '{serviceType.FullName}'.");
                            }

                            yield return () => new ServiceDescriptor(
                                serviceType,
                                serviceActivator.GetFactory(method),
                                registerAttribute.Scope);
                        }
                    }
                }
            }
        }

        private static bool CanScan(ICustomAttributeProvider customAttributeProvider, IServiceProvider serviceProvider)
        {
            return customAttributeProvider
                .GetCustomAttributes(typeof(IServiceScanningFilter), false)
                .OfType<IServiceScanningFilter>()
                .All(filter => filter.CanScan(serviceProvider));
        }
    }
}
