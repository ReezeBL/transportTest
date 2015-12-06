using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transportTest.Clusterization
{
    public interface IMetrics<T>
    {
        double Calculate(T[] a, T[] b);
        T[] GetCentroid(IList<T[]> data);
    }
}
