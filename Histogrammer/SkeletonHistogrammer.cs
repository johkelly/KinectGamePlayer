using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace CSCI598.Proj3.Histogrammer
{
    public interface SkeletonHistogrammer
    {
        List<Histogram> processSkeletons(List<Skeleton> skeletons);
    }
}
