using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace z.Autowire.Attributes
{
    /// <summary>
    /// Specifies that a type or method should be excluded or included from dependency injection registration based
    /// on a configuration setting coming from the <see cref="IConfiguration"/> service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ConfigurationBasedSelectorAttribute : Attribute, IServiceScanningFilter
    {
        private readonly string _configurationKey;
        private readonly string _enabledIfEqualsTo;

        public ConfigurationBasedSelectorAttribute(string configurationKey, string enabledIfEqualsTo)
        {
            _configurationKey = configurationKey;
            _enabledIfEqualsTo = enabledIfEqualsTo;
        }

        public bool CanScan(IServiceProvider serviceProvider)
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            string value = configuration[_configurationKey];

            return StringComparer.OrdinalIgnoreCase.Equals(value, _enabledIfEqualsTo);
        }
    }
}
