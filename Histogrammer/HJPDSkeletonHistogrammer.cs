using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3.Histogrammer
{
    public class HJPDSkeletonHistogrammer : SkeletonHistogrammer
    {
        // (Lower bound, number of bins, upper bound)
        public SortedDictionary<JointType, Tuple<double, int, double>> binDefinitions { get; set; }

        public HJPDSkeletonHistogrammer(SortedDictionary<JointType, Tuple<double, int, double>> binDefinitions)
        {
            this.binDefinitions = binDefinitions;
        }

        public List<Histogram> processSkeletons(List<Microsoft.Kinect.Skeleton> skeletons)
        {
            // Throw if no skeletons were received
            if (skeletons.Count == 0)
            {
                throw new ArgumentException("No skeletons were received - cannot process nothing");
            }
            // Prepare empty hisotgrams
            List<Histogram> histograms = new List<Histogram>();
            // Prepare empty lists to organize distances by joint
            Dictionary<JointType, List<double>> data = new Dictionary<JointType,List<double>>();
            foreach (JointType j in Enum.GetValues(typeof(JointType)))
            {
                data[j] = new List<double>();
            }
            // Process each joint in each skeleton frame
            foreach (Skeleton s in skeletons) {
                SkeletonPoint center = s.Joints[JointType.HipCenter].Position;
                foreach (Joint j in s.Joints) {
                    if (data[j.JointType] == null) {
                        data[j.JointType] = new List<double>();
                    }
                    SkeletonPoint pos = j.Position;
                    double dx = Math.Pow(pos.X - center.X, 2);
                    double dy = Math.Pow(pos.Y - center.Y, 2);
                    double dz = Math.Pow(pos.Z - center.Z, 2);
                    data[j.JointType].Add(Math.Sqrt(dx*dx + dy*dy + dz*dz));
                }
            }
            // Create a histogram for each joint, using the defined bin width and counts
            foreach (var binDefinition in binDefinitions)
            {
                histograms.Add(new Histogram(data[binDefinition.Key].ToArray(), binDefinition.Value.Item2, binDefinition.Value.Item1, binDefinition.Value.Item3));
            }
            
            return histograms;
        }

    }
}
