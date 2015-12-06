using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transportTest.Clusterization
{
    public class DataItem<T>
    {
        private T[] data;

        public DataItem(T[] input)
        {
            data = input;
        }

        public T[] Data
        {
            get { return data; }
        }       
    }
}
