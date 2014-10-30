using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3
{
    class HistogramSharePoint
    {
        private Object ListLocker = new Object();

        /// <summary>
        /// Actual location of the histogram batch reference
        /// </summary>
        private List<Histogram> histBatchRef;
        /// <summary>
        /// Location for histogram batches which need to be shared across threads.
        /// Retrieving or assigning the stored reference is threadsafe.
        /// Modifying the referenced object IS NOT threadsafe.
        /// </summary>
        public List<Histogram> histBatch
        {
            get
            {
                lock (ListLocker)
                {
                    return histBatchRef;
                }
            }
            set
            {
                lock (ListLocker)
                {
                    histBatchRef = value;
                }
            }
        }
    }
}
