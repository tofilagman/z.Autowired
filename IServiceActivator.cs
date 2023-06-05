using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace z.Autowire
{
    /// <summary>
    /// Represents a class that can construct factory methods based either on type constructors or static methods.
    /// </summary>
    public interface IServiceActivator
    {
        /// <summary>
        /// Returns a factory method based on a static method.
        /// </summary>
        Func<IServiceProvider, object?> GetFactory(MethodInfo methodInfo);

        /// <summary>
        /// Returns a factory method based on a type constructor.
        /// </summary>
        Func<IServiceProvider, object> GetFactory(Type type);
    }
}
