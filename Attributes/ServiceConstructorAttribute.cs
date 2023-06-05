using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Autowire.Attributes
{
    /// <summary>
    /// Specifies which constructor should be used for instantiation when the type is activated for
    /// dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class ServiceConstructorAttribute : Attribute
    { }
}
