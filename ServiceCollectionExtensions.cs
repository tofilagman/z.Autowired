using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace z.Autowire
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ScanAssembly(
            this IServiceCollection services,
            Assembly assembly,
            Func<Type, bool> typeFilter,
            ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Add)
        {
            services.ScanTypes(assembly.GetExportedTypes().Where(typeFilter), mergeStrategy);

            return services;
        }

        public static IServiceCollection ScanCurrentAssembly(
            this IServiceCollection services,
            ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Add)
        {
            return services.ScanAssembly(Assembly.GetCallingAssembly(), static _ => true, mergeStrategy);
        }

        public static IServiceCollection ScanTypes(
            this IServiceCollection services,
            IEnumerable<Type> types,
            ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Add)
        {
            services.TryAddSingleton<IServiceActivator>(new ServiceActivator());
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IEnumerable<Func<ServiceDescriptor>> serviceDescriptorFactoryList =
                types.SelectMany(type =>
                    ServiceScanner.ScanServiceRegistrations(type, serviceProvider).Concat(
                    ServiceScanner.ScanFactoryRegistrations(type, serviceProvider)));

            List<ServiceDescriptor> serviceDescriptors = new();
            object gate = new();

            Parallel.ForEach(
                serviceDescriptorFactoryList,
                getDescriptor =>
                {
                    ServiceDescriptor descriptor = getDescriptor();
                    lock (gate)
                        serviceDescriptors.Add(descriptor);
                });

            foreach (ServiceDescriptor serviceDescriptor in serviceDescriptors)
                MergeServiceDescriptor(services, serviceDescriptor, mergeStrategy);

            return services;
        }

        private static void MergeServiceDescriptor(
            IServiceCollection services,
            ServiceDescriptor serviceDescriptor,
            ServiceDescriptorMergeStrategy mergeStrategy)
        {
            switch (mergeStrategy)
            {
                case ServiceDescriptorMergeStrategy.Add:
                    services.Add(serviceDescriptor);
                    break;
                case ServiceDescriptorMergeStrategy.Replace:
                    services.Replace(serviceDescriptor);
                    break;
                case ServiceDescriptorMergeStrategy.TryAdd:
                    services.TryAdd(serviceDescriptor);
                    break;
                default:
                    if (services.Any(service => service.ServiceType == serviceDescriptor.ServiceType))
                        throw new ArgumentException(
                            $"The service of type {serviceDescriptor.ServiceType.FullName} has already been added.");

                    services.Replace(serviceDescriptor);
                    break;
            }
        }
    }
}
