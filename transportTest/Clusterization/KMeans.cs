using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transportTest.Clusterization
{
    public class KMeans : IClusterization<double>
    {
        #region Variables
        int clusterCount;
        IMetrics<double> metrics;
        IList<double[]> centroids;

        int maxIterationCount;
        #endregion
        private IList<double[]> GenerateStartingCentroids(IList<DataItem<double>> data)
        {
            throw new NotImplementedException();
        }
        public static IList<double[]> KMeansPPClusters(int ClusterCount, IList<DataItem<double>> data, IMetrics<double> metrics, double[] firstPoint = null)
        {
            Random rand = new Random();
            List<double[]> centroids = new List<double[]>();
            if(firstPoint == null)          
                firstPoint = data.ElementAt(rand.Next(data.Count)).Data;
            centroids.Add(firstPoint);
            for(int i = 0; i < ClusterCount - 1; ++i)
            {
                double sum = (from d in data select d.Data).Sum(d => centroids.Min(c => Math.Pow(metrics.Calculate(c, d),2)));
                double rndSum = rand.NextDouble() * sum;
                sum = 0;
                foreach(var d in (from d in data select d.Data))
                {
                    sum += centroids.Min(c => Math.Pow(metrics.Calculate(c, d), 2));
                    if(sum >= rndSum)
                    {
                        centroids.Add(d);
                        break;
                    }
                }
            }
            return centroids;
        }
        public KMeans(IList<double[]> Centroids, IMetrics<double> Metrics, int MaxIterations = 1000)
        {
            centroids = Centroids;
            metrics = Metrics;
            clusterCount = Centroids.Count;
            maxIterationCount = MaxIterations;
        }
        public KMeans(int ClusterCount, IMetrics<double> Metrics, int MaxIterations = 1000)
        {
            clusterCount = ClusterCount;
            metrics = Metrics;
            maxIterationCount = MaxIterations;
        }

        public ClusterizationResult<double> MakeClusterization(IList<DataItem<double>> data)
        {        
            int Dim = data.First().Data.Length;
            #region ForRandom
            Random r = new Random();
            double[] min = new double[Dim];
            double[] max = new double[Dim];

            for (int i = 0; i < Dim; i++)
            {
                min[i] = (from d in data
                          select d.Data[i]).Min();
                max[i] = (from d in data
                          select d.Data[i]).Max();
            }
            #endregion

            Dictionary<double[], IList<DataItem<double>>> clusterization = new Dictionary<double[], IList<DataItem<double>>>();

            #region CentroodsInit
            if (centroids == null)
            {
                for (int i = 0; i < clusterCount; i++)
                {
                    double[] v = min.Zip(max, (a, b) => r.NextDouble() * Math.Abs(b - a)).Zip(min, (a, b) => a + b).ToArray();                    
                    clusterization.Add(v, new List<DataItem<double>>());
                }               
            }
            else
            {
               foreach(var v in centroids)
                    clusterization.Add(v, new List<DataItem<double>>());
            }
            #endregion
            
            double lastCost = Double.MaxValue;
            int iterations = 0;

            while (true)
            {               
                foreach (DataItem<double> item in data)
                {
                    var candidates = from v in clusterization.Keys
                                     select new
                                     {
                                         Dist = metrics.Calculate(v, item.Data),
                                         Cluster = v
                                     };
                    double minDist = (from c in candidates
                                      select c.Dist).Min();
                    var minCandidates = from c in candidates
                                        where c.Dist == minDist
                                        select c.Cluster;
                    double[] key = minCandidates.First();
                    clusterization[key].Add(item);
                }

                double cost = 0;
                List<double[]> newMeans = new List<double[]>();
                foreach (double[] key in clusterization.Keys)
                {
                    double[] v = new double[key.Length];
                    if (clusterization[key].Count > 0)
                    {
                        v = metrics.GetCentroid((from x in clusterization[key]
                                                  select x.Data).ToArray());
                        cost += (from d in clusterization[key]
                                 select Math.Pow(metrics.Calculate(key, d.Data), 2)).Sum();
                    }
                    else
                    {
                        v = min.Zip(max, (a, b) => r.NextDouble() * Math.Abs(b - a)).Zip(min, (a, b) => a + b).ToArray();
                        Console.WriteLine("Empty cluster on iter: {0}", iterations);
                    }
                    newMeans.Add(v);
                }

                if (lastCost <= cost)
                    break;
                iterations++;
                if (iterations == maxIterationCount)
                    break;
                lastCost = cost;
                clusterization.Clear();
                newMeans.ForEach(v => clusterization.Add(v, new List<DataItem<double>>()));
            }

            return new ClusterizationResult<double>()
            {
                Centroids = new List<double[]>(clusterization.Keys),
                Clusterization = clusterization,
                Cost = lastCost
            };
        }
    }
}
