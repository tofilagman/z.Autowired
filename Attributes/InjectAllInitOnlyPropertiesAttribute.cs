using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Autowire.Attributes
{
    /// <summary>
    /// Specifies that all the init-only properties in a class should automatically be bound using dependency
    /// injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InjectAllInitOnlyPropertiesAttribute : Attribute
    { }
}
