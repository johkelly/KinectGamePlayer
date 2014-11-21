using CSCI598.Proj3.Histogrammer;
using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3
{
    public class SkeletonFrameWindowProcessor
    {
        public const int WINDOW_SIZE = 10;
        private List<Skeleton> skeletons = new List<Skeleton>();
        private HistogramSharePoint hsp;
        private SkeletonHistogrammer histogrammer;

        public SkeletonFrameWindowProcessor(HistogramSharePoint hsp, SkeletonHistogrammer histogrammer)
        {
            this.hsp = hsp;
            this.histogrammer = histogrammer;
        }

        public void handleNewSkeleton(object sender, Microsoft.Kinect.Skeleton e)
        {
            while (skeletons.Count >= WINDOW_SIZE)
            {
                skeletons.RemoveAt(skeletons.Count - 1);
            }
            skeletons.Insert(0, e);
            if (skeletons.Count == WINDOW_SIZE)
            {
                hsp.histBatch = histogrammer.processSkeletons(skeletons);
            }
        }

    }
}
