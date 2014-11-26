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
    /// The RAD representation for skeleton histograms uses a singular joint as a center, and then an arbitrary list of joints
    /// around it to form n distance and angle measures, where n is the size of the joint list.
    /// </summary>
    public class RADSkeletonHistogrammer : SkeletonHistogrammer
    {
        public static int RADBinCount = 20;
        public static readonly List<JointType> DefaultJointList = new List<JointType>() { JointType.ElbowRight, JointType.HandRight, JointType.HandLeft, JointType.ElbowLeft };
        public static readonly JointType DefaultCenterJoint = JointType.Head;

        public List<BinDefinition> binDefinitions { get; set; }
        public List<JointType> jointList { get; set; }

        /// <summary>
        /// Extract the relevant measures from the raw skeleton data.
        /// </summary>
        /// <param name="jointList"></param>
        /// <param name="skeletons"></param>
        /// <returns>A list of lists of doubles. Each list represents one measure for RAD (i.e. a distance or an angle)
        /// and has one entry for each valid frame in the skeleton data</returns>
        public static List<List<double>> prepareData(List<JointType> jointList, List<Skeleton> skeletons)
        {
            List<List<double>> data = new List<List<double>>();
            for (int i = 0; i < jointList.Count; ++i)
            {
                // Add a list for distance measurements and a list for angle measurements
                data.Add(new List<double>());
                data.Add(new List<double>());
            }
            foreach (Skeleton skeleton in skeletons)
            {
                SkeletonPoint center = skeleton.Joints[DefaultCenterJoint].Position;
                Point3D centerPoint = new Point3D(center.X, center.Y, center.Z);
                List<Point3D> points = new List<Point3D>();
                foreach (JointType jointType in jointList)
                {
                    SkeletonPoint point = skeleton.Joints[jointType].Position;
                    points.Add(new Point3D(point.X, point.Y, point.Z));
                }
                for (int i = 0; i < jointList.Count; ++i)
                {
                    data[2*i].Add(centerPoint.difference(points[i]).magnitude());
                    data[2*i + 1].Add(centerPoint.angleMadeWith(points[i], points[(i + 1) % jointList.Count]));
                }
            }
            return data;
        }

        /// <summary>
        /// Discover the proper boundaries for the eventual histograms of the skeleton data.
        /// </summary>
        /// <param name="jointList"></param>
        /// <param name="skeletonBatch"></param>
        /// <returns>A list of BinDefinition objects representing the boundaries of the bins of each metric's histogram.</returns>
        public static List<BinDefinition> binDefinitionsFor(List<JointType> jointList, List<List<Skeleton>> skeletonBatch)
        {
            // Bin definitions begin at numeric extremes
            List<BinDefinition> binDefinitions = new List<BinDefinition>();
            for (int i = 0; i < jointList.Count; ++i)
            {
                BinDefinition extremeBinDef1 = new BinDefinition()
                {
                    lowerBound = double.MaxValue,
                    upperBound = double.MinValue,
                    numBins = RADBinCount
                };
                binDefinitions.Add(extremeBinDef1);
                BinDefinition extremeBinDef2 = new BinDefinition()
                {
                    lowerBound = double.MaxValue,
                    upperBound = double.MinValue,
                    numBins = RADBinCount
                };
                binDefinitions.Add(extremeBinDef2);
            }
            // Look at the preprocessed data for each list of skeletons and update the bin definitions appropriately
            foreach (List<Skeleton> skeletons in skeletonBatch)
            {
                List<List<double>> data = prepareData(jointList, skeletons);
                for (int i = 0; i < binDefinitions.Count; ++i) {
                    BinDefinition binDef = binDefinitions[i];
                    binDef.lowerBound = Math.Min(binDef.lowerBound, data[i].Min());
                    binDef.upperBound = Math.Max(binDef.upperBound, data[i].Max());
                    binDefinitions[i] = binDef;
                }
            }

            return binDefinitions;
        }

        /// <summary>
        /// BinDefinitions can be acquired from a static method on the RADSkeletonHistogrammer class.
        /// </summary>
        /// <param name="jointList"></param>
        /// <param name="binDefinitions"></param>
        public RADSkeletonHistogrammer(List<JointType> jointList, List<BinDefinition> binDefinitions)
        {
            this.jointList = jointList;
            this.binDefinitions = binDefinitions;
        }

        public List<Histogram> processSkeletons(List<Skeleton> skeletons)
        {
            List<List<double>> data = prepareData(jointList, skeletons);
            List<Histogram> histograms = new List<Histogram>();
            for (int i = 0; i < binDefinitions.Count; ++i)
            {
                try
                {
                    histograms.Add(new Histogram(data[i].ToList(), binDefinitions[i].numBins, binDefinitions[i].lowerBound, binDefinitions[i].upperBound));
                }
                catch
                {
                    histograms = null;
                    break;
                }
            }
            return histograms;
        }
    }
}
