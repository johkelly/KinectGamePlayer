using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3.Histogrammer
{
    /// <summary>
    /// HJPDSkeletonHistogrammer has one distance metric for each joint in the skeleton relative to a central joint.
    /// </summary>
    public class HJPDSkeletonHistogrammer : SkeletonHistogrammer
    {
        public const int HJPDBinCount = 20;

        public SortedDictionary<JointType, BinDefinition> binDefinitions { get; set; }

        /// <summary>
        /// Bin deifnitions can be acquired from a static method on the HJPDSkeletonHistogrammer class.
        /// </summary>
        /// <param name="binDefinitions"></param>
        public HJPDSkeletonHistogrammer(SortedDictionary<JointType, BinDefinition> binDefinitions)
        {
            this.binDefinitions = binDefinitions;
        }

        /// <summary>
        /// Discover the proper histogram bounds for the provided skeleton data.
        /// </summary>
        /// <param name="skeletonBatch"></param>
        /// <returns>A mapping of JointType -> BinDefintion defining the boundaries of the bins for each joint's histogram.</returns>
        public static SortedDictionary<JointType, BinDefinition> binDefinitionsFor(List<List<Skeleton>> skeletonBatch)
        {
            SortedDictionary<JointType, BinDefinition> binDefinitions = null;
            foreach (List<Skeleton> skeletons in skeletonBatch)
            {
                SortedDictionary<JointType, BinDefinition> binDefs = new SortedDictionary<JointType, BinDefinition>();
                foreach (var dataset in prepareData(skeletons))
                {
                    BinDefinition binDef = new BinDefinition();
                    binDef.lowerBound = dataset.Value.Min();
                    binDef.upperBound = dataset.Value.Max();
                    binDef.numBins = HJPDBinCount;
                    binDefs[dataset.Key] = binDef;
                }
                if (binDefinitions == null)
                {
                    binDefinitions = binDefs;
                }
                else
                {
                    binDefinitions = combineDefinitions(binDefinitions, binDefs);
                }
            }
            return binDefinitions;
        }

        /// <summary>
        /// Convenience method to combine bin definitions.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>A mapping of JointType -> BinDefintion, where the lower and upper bounds of each is the lower and higher for each bin between left and right respectively</returns>
        public static SortedDictionary<JointType, BinDefinition> combineDefinitions(SortedDictionary<JointType, BinDefinition> left, SortedDictionary<JointType, BinDefinition> right)
        {
            if (!left.Keys.SequenceEqual(right.Keys))
            {
                throw new ArgumentException("Bin Definition maps disagree on the included joint types");
            }
            SortedDictionary<JointType, BinDefinition> retval = new SortedDictionary<JointType, BinDefinition>();
            foreach (JointType key in left.Keys)
            {
                if (left[key].numBins != right[key].numBins)
                {
                    throw new ArgumentException("Bin Definition maps disagree on the number of bins for " + key);
                }
                BinDefinition binDef = new BinDefinition();
                binDef.lowerBound = Math.Min(left[key].lowerBound, right[key].lowerBound);
                binDef.upperBound = Math.Max(left[key].upperBound, right[key].upperBound);
                binDef.numBins = left[key].numBins;
                retval[key] = binDef;
            }
            return retval;
        }

        /// <summary>
        /// Preprocess the data into a mapping of JointType -> distances
        /// </summary>
        /// <param name="skeletons"></param>
        /// <returns>A mapping of JointType -> distances, where each list of distances contains the distance of the associated joint to the central joint at each frame.</returns>
        private static Dictionary<JointType, List<double>> prepareData(List<Skeleton> skeletons)
        {
            // Throw if no skeletons were received
            if (skeletons.Count == 0)
            {
                throw new ArgumentException("No skeletons were received - cannot process nothing");
            }
            // Prepare empty lists to organize distances by joint
            Dictionary<JointType, List<double>> data = new Dictionary<JointType, List<double>>();
            foreach (JointType j in Enum.GetValues(typeof(JointType)))
            {
                data[j] = new List<double>();
            }
            // Process each joint in each skeleton frame
            foreach (Skeleton s in skeletons)
            {
                SkeletonPoint center = s.Joints[JointType.HipCenter].Position;
                foreach (Joint j in s.Joints)
                {
                    if (data[j.JointType] == null)
                    {
                        data[j.JointType] = new List<double>();
                    }
                    SkeletonPoint pos = j.Position;
                    double dx = Math.Pow(pos.X - center.X, 2);
                    double dy = Math.Pow(pos.Y - center.Y, 2);
                    double dz = Math.Pow(pos.Z - center.Z, 2);
                    data[j.JointType].Add(Math.Sqrt(dx * dx + dy * dy + dz * dz));
                }
            }
            return data;
        }

        public List<Histogram> processSkeletons(List<Microsoft.Kinect.Skeleton> skeletons)
        {
            // Prepare empty hisotgrams
            List<Histogram> histograms = new List<Histogram>();
            // Get raw data
            Dictionary<JointType, List<double>> data = prepareData(skeletons);
            // Create a histogram for each joint, using the defined bin width and counts
            foreach (var binDefinition in binDefinitions)
            {
                histograms.Add(new Histogram(data[binDefinition.Key].ToArray(), binDefinition.Value.numBins, binDefinition.Value.lowerBound, binDefinition.Value.upperBound));
            }
            
            return histograms;
        }
    }
}
