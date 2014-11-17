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
        public const int WINDOW_SIZE = 50;
        private List<Skeleton> skeletons = new List<Skeleton>(WINDOW_SIZE);
        private HistogramSharePoint hsp;
        private SkeletonHistogrammer histogrammer;

        public SkeletonFrameWindowProcessor(HistogramSharePoint hsp)
        {
            this.hsp = hsp;
            string[] binDefLines = System.IO.File.ReadAllLines(@"dataset_bounds_path.txt");
            SortedDictionary<JointType, Tuple<double, int, double>> binDefinitions = new SortedDictionary<JointType, Tuple<double, int, double>>();
            foreach (string binDef in binDefLines)
            {
                string[] vals = binDef.Split();
                binDefinitions[(JointType)int.Parse(vals[0])] = new Tuple<double, int, double>(double.Parse(vals[1]), int.Parse(vals[2]), double.Parse(vals[3]));
            }
            this.histogrammer = new HJPDSkeletonHistogrammer(binDefinitions);
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
