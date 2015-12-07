using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transportTest.Interpolation
{
    public class RPropInterpolator
    {
        double[] xWeights, yWeights;
        const double a = 1, u = 1;
        public RPropInterpolator(double[] xVals, double[] yVals, double[] fVals, int n)
        {
            Random rand = new Random();

            xWeights = new double[n];
            yWeights = new double[n];
            for (int i = 0; i < n; i++)
            {
                xWeights[i] = rand.NextDouble();
                yWeights[i] = rand.NextDouble();
            }
            for (int i = 0; i < xVals.Length; i++)
                teach(xVals[i], yVals[i], fVals[i]);
        }

        private void teach(double x, double y, double f)
        {

        }

        public double result(double x, double y)
        {
            int i = 0, j=0;
            return xWeights.Sum(e => e * Math.Pow(x, i++)) + yWeights.Sum(e => e * Math.Pow(y, i));
        }
    }
}
