using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace CSCI598.Proj3.Histogrammer
{
    public interface SkeletonHistogrammer
    {
        /// <summary>
        /// Given a list of skeletons, produce a list of histograms for the implementation's representation.
        /// </summary>
        /// <param name="skeletons"></param>
        /// <returns>A list of histograms of arbitrary size</returns>
        List<Histogram> processSkeletons(List<Skeleton> skeletons);
    }
}
