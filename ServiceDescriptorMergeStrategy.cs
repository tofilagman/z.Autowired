using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Autowire
{
    public enum ServiceDescriptorMergeStrategy
    {
        /// <summary>
        /// Ignore conflicts and add conflicting service descriptors to the collection.
        /// </summary>
        Add,
        /// <summary>
        /// Remove the first service descriptors with the same service type in case of conflict, then add the new
        /// service descriptor to the collection.
        /// </summary>
        Replace,
        /// <summary>
        /// Add a service descriptor to the collection only if the service type hasn't already been registered.
        /// </summary>
        TryAdd,
        /// <summary>
        /// Throw an exception in case of conflicting service descriptors.
        /// </summary>
        Throw
    }
}
