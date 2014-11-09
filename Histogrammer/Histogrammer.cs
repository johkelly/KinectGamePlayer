using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3.Histogrammer
{
    public class KinectInstanceHistogrammer
    {
        public static List<Histogram> HJPDHistogram(List<Skeleton> skeletons, SortedDictionary<JointType, Tuple<double, int, double>> binDefinitions)
        {
            SkeletonHistogrammer histogrammer = new HJPDSkeletonHistogrammer(binDefinitions);
            return histogrammer.processSkeletons(skeletons);
        }
    }
}
