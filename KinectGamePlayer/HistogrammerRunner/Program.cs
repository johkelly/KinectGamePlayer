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
            SortedDictionary<JointType, Tuple<double, int, double>> binDefinitions = new SortedDictionary<JointType, Tuple<double, int, double>>();
            foreach (JointType type in Enum.GetValues(typeof(JointType)))
            {
                binDefinitions[type] = new Tuple<double, int, double>(double.MaxValue, 20, double.MinValue);
            }
            // Process each input file in the data directory for skeleton and histogram data
            foreach (string iFile in Directory.GetFiles("data", "*.txt"))
            {
                System.Console.WriteLine("Read: " + iFile);
                List<Skeleton> skeletons = SkeletonListSerializer.makeFromeFile(iFile);
                foreach (Skeleton s in skeletons)
                {
                    SkeletonPoint center = s.Joints[JointType.HipCenter].Position;
                    foreach (Joint j in s.Joints)
                    {
                        SkeletonPoint pos = j.Position;
                        double dx = Math.Pow(pos.X - center.X, 2);
                        double dy = Math.Pow(pos.Y - center.Y, 2);
                        double dz = Math.Pow(pos.Z - center.Z, 2);
                        double dist = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                        Tuple<double, int, double> binDef = binDefinitions[j.JointType];
                        binDefinitions[j.JointType] = new Tuple<double,int,double>(Math.Min(binDef.Item1, dist), binDefinitions[j.JointType].Item2, Math.Max(binDef.Item3, dist));
                    }
                }
                // Add the skeleton sequence and class assignment to proper collections
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

            // Adjust the lower and upper bounds by a very small amount because Math.Net will throw an exception
            // when a Histogram is constructed with explicit data and bounds where a data value is precisely
            // equal to a bound. Seriously, WTF.
            foreach (JointType j in binDefinitions.Keys.ToList())
            {
                Tuple<double, int, double> unfixed = binDefinitions[j];
                binDefinitions[j] = new Tuple<double,int,double>(unfixed.Item1.Decrement(), unfixed.Item2, unfixed.Item3.Increment());
            }
            // Histogram each skeleton sequence, and write it out to LIBSVM format
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
                boundsFile.WriteLine((int)binDef.Key + " " + binDef.Value.Item1 + " " + binDef.Value.Item2 + " " + binDef.Value.Item3);
            }
            System.Console.WriteLine("Wrote training bounds");
            boundsFile.Close();

            System.Console.ReadKey(true);
        }
    }
}
