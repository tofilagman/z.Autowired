using Microsoft.Extensions.DependencyInjection;

namespace z.Autowire.Attributes
{
    /// <summary>
    /// Specifies that a method is a factory producing a service that can be used for dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class BeanAttribute : Attribute
    {
        public BeanAttribute(ServiceLifetime scope = ServiceLifetime.Transient)
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
