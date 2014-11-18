using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3.Histogrammer
{
    public class RADSkeletonHistogrammer : SkeletonHistogrammer
    {
        private const int RAD_BIN_COUNT = 20;

        public List<BinDefinition> binDefinitions { get; set; }
        public List<JointType> jointList { get; set; }

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
                SkeletonPoint center = skeleton.Joints[JointType.HipCenter].Position;
                Point3D centerPoint = new Point3D(center.X, center.Y, center.Z);
                List<Point3D> points = new List<Point3D>();
                foreach (JointType jointType in jointList)
                {
                    SkeletonPoint point = skeleton.Joints[jointType].Position;
                    points.Add(new Point3D(point.X, point.Y, point.Z));
                }
                for (int i = 0; i < jointList.Count; ++i)
                {
                    data[i].Add(centerPoint.difference(points[i]).magnitude());
                    data[i + 1].Add(centerPoint.angleMadeWith(points[i], points[(i + 1) % jointList.Count]));
                }
            }
            return data;
        }

        public static List<BinDefinition> binDefinitionsFor(List<JointType> jointList, List<List<Skeleton>> skeletonBatch)
        {
            List<BinDefinition> binDefinitions = new List<BinDefinition>();
            for (int i = 0; i < jointList.Count; ++i)
            {
                BinDefinition extremeBinDef = new BinDefinition();
                extremeBinDef.lowerBound = double.MaxValue;
                extremeBinDef.upperBound = double.MinValue;
                extremeBinDef.numBins = RAD_BIN_COUNT;
                binDefinitions.Add(extremeBinDef);
                binDefinitions.Add(extremeBinDef);
            }
            foreach (List<Skeleton> skeletons in skeletonBatch)
            {
                List<List<double>> data = prepareData(jointList, skeletons);
                for (int i = 0; i < jointList.Count; ++i) {
                    BinDefinition binDef = binDefinitions[i];
                    binDef.lowerBound = Math.Min(binDef.lowerBound, data[i].Min());
                    binDef.upperBound = Math.Max(binDef.upperBound, data[i].Max());
                    binDefinitions[i] = binDef;
                }
            }
            return binDefinitions;
        }

        public RADSkeletonHistogrammer(List<JointType> jointList, List<BinDefinition> binDefinitions)
        {
            this.jointList = jointList;
            this.binDefinitions = binDefinitions;
        }

        public List<Histogram> processSkeletons(List<Skeleton> skeletons)
        {
            List<List<double>> data = prepareData(jointList, skeletons);
            List<Histogram> histograms = new List<Histogram>();
            for (int i = 0; i < jointList.Count; ++i)
            {
                histograms.Add(new Histogram(data[i].ToList(), binDefinitions[i].numBins, binDefinitions[i].lowerBound, binDefinitions[i].upperBound));
            }
            return histograms;
        }
    }
}
