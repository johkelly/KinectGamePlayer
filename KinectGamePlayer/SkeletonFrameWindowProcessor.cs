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
    /// <summary>
    /// Converts a stream of skeleton data into histograms over a sliding window of that data.
    /// </summary>
    public class SkeletonFrameWindowProcessor
    {
        // Roughly 1/3 of a second, Kinect produces 30 frames/second on average
        public static int WindowSize = 10;

        private List<Skeleton> skeletons = new List<Skeleton>();
        private HistogramSharePoint hsp;
        private SkeletonHistogrammer histogrammer;

        public SkeletonFrameWindowProcessor(HistogramSharePoint hsp, SkeletonHistogrammer histogrammer)
        {
            this.hsp = hsp;
            this.histogrammer = histogrammer;
            // Construct a reader to get skeleton data from the Kinect, and listen for its udpates
            RawSkeletonReader reader = new RawSkeletonReader();
            reader.rawSkeletonReady += this.handleNewSkeleton;
        }

        /// <summary>
        /// Update the sliding window of histograms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void handleNewSkeleton(object sender, Microsoft.Kinect.Skeleton e)
        {
            while (skeletons.Count >= WindowSize)
            {
                skeletons.RemoveAt(skeletons.Count - 1);
            }
            skeletons.Insert(0, e);
            if (skeletons.Count == WindowSize)
            {
                hsp.histBatch = histogrammer.processSkeletons(skeletons);
            }
        }

    }
}
