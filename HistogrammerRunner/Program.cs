using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using CSCI598.Proj3.Histogrammer;

namespace HistogrammerRunner
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Assemble empty skeleton data and histogram data containers with default values
            List<List<Skeleton>> allSkeletons = new List<List<Skeleton>>();
            List<int> classes = new List<int>();
            foreach (string iFile in Directory.GetFiles("data", "*.txt"))
            {
                System.Console.WriteLine("Read: " + iFile);
                List<Skeleton> skeletons = SkeletonListSerializer.makeFromeFile(iFile);
                allSkeletons.Add(skeletons);  
                if (iFile.Contains("Norm"))
                {
                    classes.Add(0);
                }
                else if (iFile.Contains("LShoulder"))
                {
                    classes.Add(1);
                }
                else if (iFile.Contains("RShoulder"))
                {
                    classes.Add(2);
                }
                else if (iFile.Contains("LHip"))
                {
                    classes.Add(3);
                }
                else if (iFile.Contains("RHip"))
                {
                    classes.Add(4);
                }
                else
                {
                    System.Console.WriteLine("Missed " + iFile);
                }
            }

            List<BinDefinition> binDefinitions;
            List<List<Histogram>> histograms;
            makeHistogramsWithRAD(allSkeletons, RADSkeletonHistogrammer.DefaultJointList, out histograms, out binDefinitions);
            StreamWriter boundsFile = new StreamWriter(PipelineConstants.SVMBoundsFile);
            for (int i = 0; i < binDefinitions.Count; ++i)
            {
                boundsFile.WriteLine(i + " " + binDefinitions[i].lowerBound + " " + binDefinitions[i].numBins + " " + binDefinitions[i].upperBound);
            }
            System.Console.WriteLine("Wrote bounds to file");
            boundsFile.Close();

            // Write the histogram attributes to a file
            StreamWriter trainingFile = new StreamWriter(PipelineConstants.SVMFeaturesFile);
            for (int instnum = 0; instnum < allSkeletons.Count; ++instnum)
            {
                int attribute = 1;
                StringBuilder builder = new StringBuilder();
                foreach (Histogram h in histograms[instnum])
                {
                    double frameCount = 0;
                    for (int i = 0; i < h.BucketCount; ++i)
                    {
                        frameCount += h[i].Count;
                    }
                    for (int i = 0; i < h.BucketCount; ++i)
                    {
                        builder.Append(attribute.ToString() + ":" + h[i].Count / frameCount + " ");
                        ++attribute;
                    }
                }
                trainingFile.WriteLine(classes[instnum].ToString() + " " + builder.ToString());
            }
            System.Console.WriteLine("Wrote training file");
            trainingFile.Close();

            System.Console.ReadKey(true);
        }

        private static void makeHistogramsWithHJPD(List<List<Skeleton>> allSkeletons, out List<List<Histogram>> histograms, out SortedDictionary<JointType, BinDefinition> binDefinitions)
        {
            histograms = new List<List<Histogram>>();
            binDefinitions = HJPDSkeletonHistogrammer.binDefinitionsFor(allSkeletons);
            // Adjust the lower and upper bounds by a very small amount because Math.Net will throw an exception
            // when a Histogram is constructed with explicit data and bounds where a data value is precisely
            // equal to a bound. Seriously, WTF.
            foreach (JointType j in binDefinitions.Keys.ToList())
            {
                BinDefinition binDef = binDefinitions[j];
                binDef.lowerBound = binDef.lowerBound.Decrement();
                binDef.upperBound = binDef.upperBound.Increment();
                binDefinitions[j] = binDef;
            }
            SkeletonHistogrammer histogrammer = new HJPDSkeletonHistogrammer(binDefinitions);
            for (int instNum = 0; instNum < allSkeletons.Count; ++instNum)
            {
                if (allSkeletons[instNum].Count == 0)
                {
                    continue;
                }
                histograms.Add(histogrammer.processSkeletons(allSkeletons[instNum]));
            }
        }

        private static void makeHistogramsWithRAD(List<List<Skeleton>> allSkeletons, List<JointType> jointList, out List<List<Histogram>> histograms, out List<BinDefinition> binDefinitions)
        {
            histograms = new List<List<Histogram>>();
            binDefinitions = RADSkeletonHistogrammer.binDefinitionsFor(jointList, allSkeletons);
            // Adjust the lower and upper bounds by a very small amount because Math.Net will throw an exception
            // when a Histogram is constructed with explicit data and bounds where a data value is precisely
            // equal to a bound. Seriously, WTF.
            foreach (BinDefinition binDef in binDefinitions)
            {
                binDef.lowerBound = binDef.lowerBound.Decrement();
                binDef.upperBound = binDef.upperBound.Increment();
            }
            SkeletonHistogrammer histogrammer = new RADSkeletonHistogrammer(jointList, binDefinitions);
            foreach (List<Skeleton> skeletons in allSkeletons)
            {
                if (skeletons.Count == 0)
                {
                    continue;
                }
                histograms.Add(histogrammer.processSkeletons(skeletons));
            }
        }
    }
}
