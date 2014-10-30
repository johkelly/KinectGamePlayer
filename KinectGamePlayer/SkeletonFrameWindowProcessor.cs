using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3
{
    class SkeletonFrameWindowProcessor
    {
        private const int WINDOW_SIZE = 10;
        private List<Skeleton> skeletons = new List<Skeleton>(WINDOW_SIZE);
        private HistogramSharePoint hsp;

        public SkeletonFrameWindowProcessor(HistogramSharePoint hsp)
        {
            this.hsp = hsp;
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
                List<List<double>> rawData = new List<List<double>>();
                foreach (var _ in Enum.GetNames(typeof(JointType)))
                {
                    rawData.Add(new List<double>());
                }
                foreach (Skeleton s in skeletons)
                {
                    SkeletonPoint center = s.Joints[JointType.HipCenter].Position;
                    foreach (Joint j in s.Joints)
                    {
                        SkeletonPoint pos = j.Position;
                        double dx = Math.Pow(pos.X - center.X, 2);
                        double dy = Math.Pow(pos.Y - center.Y, 2);
                        double dz = Math.Pow(pos.Z - center.Z, 2);
                        rawData[(int)j.JointType].Add(Math.Sqrt(dx + dy + dz));
                    }
                }
                List<Histogram> histograms = new List<Histogram>();
                foreach (List<double> l in rawData)
                {
                    histograms.Add(new Histogram(l.ToArray(), 10));
                }
                hsp.histBatch = histograms;
            }
        }
    }
}
