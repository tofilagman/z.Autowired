using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Autowire.Attributes
{
    /// <summary>
    /// Specifies that a class is a service that can be used for dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceLifetime scope = ServiceLifetime.Transient)
        {
            Scope = scope;
        }

        /// <summary>
        /// The lifetime of the service.
        /// </summary>
        public ServiceLifetime Scope { get; }

        /// <summary>
        /// The type of the service.
        /// </summary>
        public Type? ServiceType { get; set; }
    }
}
