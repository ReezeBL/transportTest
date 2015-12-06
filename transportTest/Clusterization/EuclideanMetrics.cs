using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transportTest.Clusterization
{
    public class EuclideanMetrics : IMetrics<double>
    {
        public double Calculate(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vectors' dimensions are different");
            return Math.Sqrt(a.Zip(b, (v1, v2) => (v2 - v1) * (v2 - v1)).Sum());
        }

        public double[] GetCentroid(IList<double[]> data)
        {
            double[] center = new double[data.First().Length];
            foreach(var v in data)
            {
                center = center.Zip(v, (a, b) => a + b).ToArray();
            }
            return center.Zip(center, (a, b) => a / data.Count).ToArray();
        }
    }
}
