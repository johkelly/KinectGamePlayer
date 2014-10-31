using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histogrammer;
using System.IO;
using MathNet.Numerics.Statistics;

namespace HistogrammerRunner
{
    class Program
    {
        public static void Main(string[] args)
        {
            StreamWriter trainingFile = new StreamWriter("train.txt");
            foreach (string iFile in Directory.GetFiles("data", "*.txt"))
            {
                System.Console.WriteLine(iFile);
                List<Histogram> hgrams = Histogrammer.KinectInstanceHistogrammer.HJPDHistogram(iFile, -.001, 3, 10);
                // to file
                int c = 0;
                if (iFile.Contains("Default")) {
                    c = 0;
                }
                else if (iFile.Contains("Left"))
                {
                    c = 1;
                }
                else if (iFile.Contains("Right"))
                {
                    c = 2;
                }
                else if (iFile.Contains("LHip"))
                {
                    c = 3;
                }
                else if (iFile.Contains("RHip"))
                {
                    c = 4;
                }
                else if (iFile.Contains("Push"))
                {
                    c = 5;
                }
                StringBuilder builder = new StringBuilder();
                int currIdx = 1;
                foreach (Histogram h in hgrams)
                {
                    for (int i = 0; i < h.BucketCount; ++i) {
                        builder.Append(currIdx.ToString() + ":" + (h[i].Count + " "));
                        currIdx++;
                    }
                }
                trainingFile.WriteLine(c + " " + builder.ToString());
            }
            System.Console.ReadKey(true);
        }
    }
}
