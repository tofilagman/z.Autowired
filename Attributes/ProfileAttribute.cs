using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace z.Autowire.Attributes
{
    /// <summary>
    /// Specifies that a type or method should be excluded or included from dependency injection registration based on
    /// the current environment specified throught the <see cref="IHostEnvironment"/> service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ProfileAttribute : Attribute, IServiceScanningFilter
    {
        public string[]? Enabled { get; set; }

        public string[]? Disabled { get; set; }

        public bool CanScan(IServiceProvider serviceProvider)
        {
            string environmentName = serviceProvider.GetRequiredService<IHostEnvironment>().EnvironmentName;

            bool enabled = true;
            if (Enabled != null)
                enabled &= Enabled.Contains(environmentName, StringComparer.OrdinalIgnoreCase);

            if (Disabled != null)
                enabled &= !Disabled.Contains(environmentName, StringComparer.OrdinalIgnoreCase);

            return enabled;
        }
    }
}
