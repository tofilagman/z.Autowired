using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Autowire
{
    public interface IServiceScanningFilter
    {
        bool CanScan(IServiceProvider serviceProvider);
    }
}
