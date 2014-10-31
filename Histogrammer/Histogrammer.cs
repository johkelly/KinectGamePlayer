using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histogrammer
{
    public class KinectInstanceHistogrammer
    {
        public static List<Histogram> HJPDHistogram(string instanceFileName, double lowBound, double highBound, int bins)
        {
            const int NUM_SKEL_JOINTS = 20;
            string[] lines = System.IO.File.ReadAllLines(instanceFileName);
            Dictionary<int, List<double>> jointBuckets = new Dictionary<int, List<double>>();
            for (int i = 0; i < NUM_SKEL_JOINTS; ++i)
            {
                jointBuckets[i] = new List<double>();
            }
            int uniqueFrames = 0;
            // Increment by the number of kinect joints
            for (int i = 0; i < lines.Count(); i += 20) {
                String[] cLine = lines[(int)JointType.SpineBase].Split();
                // Stop before we process the last frame with 20 joint entries
                // Protects against incomplete frames and frames with incomplete final joints
                if ((lines.Count() - i) <= 20)
                {
                    break;
                }
                Point3D center = new Point3D{
                    X = Convert.ToDouble(cLine[2]),
                    Y = Convert.ToDouble(cLine[3]),
                    Z = Convert.ToDouble(cLine[4])
                };
                for (int j = 0; j < 20; ++j)
                {
                    String[] jLine = lines[i+j].Split();
                    Point3D joint = new Point3D
                    {
                        X = Convert.ToDouble(jLine[2]),
                        Y = Convert.ToDouble(jLine[3]),
                        Z = Convert.ToDouble(jLine[4])
                    };
                    Point3D diff = center.difference(joint);
                    jointBuckets[j].Add(diff.magnitude());
                }
                ++uniqueFrames;
            }
            List<Histogram> retVal = new List<Histogram>();
            foreach (KeyValuePair<int, List<double>> dictEntry in jointBuckets)
            {
                retVal.Add(new Histogram(dictEntry.Value, bins, lowBound, highBound));
            }
            return retVal;
        }
    }
}
