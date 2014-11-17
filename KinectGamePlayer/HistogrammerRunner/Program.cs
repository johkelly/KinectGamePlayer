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
            SortedDictionary<JointType, BinDefinition> binDefinitions;
            foreach (string iFile in Directory.GetFiles("data", "*.txt"))
            {
                System.Console.WriteLine("Read: " + iFile);
                List<Skeleton> skeletons = SkeletonListSerializer.makeFromeFile(iFile);
                allSkeletons.Add(skeletons);  
                if (iFile.Contains("Default"))
                {
                    classes.Add(0);
                }
                else if (iFile.Contains("Left"))
                {
                    classes.Add(1);
                }
                else if (iFile.Contains("Right"))
                {
                    classes.Add(2);
                }
                else if (iFile.Contains("LHip"))
                {
                    classes.Add(4);
                }
                else if (iFile.Contains("RHip"))
                {
                    classes.Add(5);
                }
                else if (iFile.Contains("Push"))
                {
                    classes.Add(6);
                }
            }

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
            StreamWriter trainingFile = new StreamWriter("train.txt");
            for (int instNum = 0; instNum < allSkeletons.Count; ++instNum)
            {
                int attribute = 1;
                if (allSkeletons[instNum].Count == 0)
                {
                    continue;
                }
                List<Histogram> histograms = histogrammer.processSkeletons(allSkeletons[instNum]);
                StringBuilder builder = new StringBuilder();
                foreach (Histogram h in histograms)
                {
                    for (int i = 0; i < h.BucketCount; ++i)
                    {
                        builder.Append(attribute.ToString() + ":" + h[i].Count + " ");
                        ++attribute;
                    }
                }
                trainingFile.WriteLine(classes[instNum].ToString() + " " + builder.ToString());
            }
            System.Console.WriteLine("Wrote training file");
            trainingFile.Close();
            // Write the histogram bounds to a similar file
            StreamWriter boundsFile = new StreamWriter("train.bounds.txt");
            foreach (var binDef in binDefinitions)
            {
                boundsFile.WriteLine((int)binDef.Key + " " + binDef.Value.lowerBound + " " + binDef.Value.numBins + " " + binDef.Value.upperBound);
            }
            System.Console.WriteLine("Wrote training bounds");
            boundsFile.Close();

            System.Console.ReadKey(true);
        }
    }
}
