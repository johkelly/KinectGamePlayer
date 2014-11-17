using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3.Histogrammer
{
    public class BinDefinition
    {
        public double lowerBound { get; set; }
        public double upperBound { get; set; }
        public int numBins { get; set; }
        public BinDefinition() {
            lowerBound = double.MaxValue;
            upperBound = double.MinValue;
        }
    }
}
