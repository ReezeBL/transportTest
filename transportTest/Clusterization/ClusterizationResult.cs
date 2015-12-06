using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transportTest.Clusterization
{
    public class ClusterizationResult<T>
    {
        public IList<T[]> Centroids { get; set; }
        public IDictionary<T[], IList<DataItem<T>>> Clusterization { get; set; }
        public double Cost { get; set; }
    }
}
