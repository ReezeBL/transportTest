using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transportTest.Clusterization
{
    public interface IClusterization <T>
    {
        ClusterizationResult<T> MakeClusterization(IList<DataItem<T>> data);
    }
}
