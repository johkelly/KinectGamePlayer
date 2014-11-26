using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3.Histogrammer
{
    /// <summary>
    /// Convenience class for writing skeleton data to and from files.
    /// </summary>
    public class SkeletonListSerializer
    {
        public static List<Skeleton> makeFromeFile(string filename)
        {
            // Read in all lines
            string[] lines = System.IO.File.ReadAllLines(filename);
            // If the file was empty, quit early
            if (lines.Length == 0)
            {
                return new List<Skeleton>();
            }
            // Prepare empty skeletons
            List<Skeleton> skeletons = new List<Skeleton>();
            // Process lines 20 at a time, dropping any incomplete frames as a result
            int NUM_JOINTS = Enum.GetNames(typeof(JointType)).Length;
            for (int i = 0; i <= lines.Length-20; i += NUM_JOINTS)
            {
                // Create a new Skeleton, assume it will be reconstructed okay
                Skeleton s = new Skeleton();
                bool skeletonOkay = true;
                // Walk the batch of input lines, one per joint for this frame
                for (int j = 0; j < NUM_JOINTS; ++j)
                {
                    // Tokenize the line
                    string[] jInfo = lines[i + j].Split();
                    // If the line of info for the joint is not hte proper length,
                    // flag this skeleton as bad and break
                    if (jInfo.Length != 5)
                    {
                        skeletonOkay = false;
                        break;
                    }
                    // Capture the relevant joint reference by converting the integer-string to a JointType enum
                    Joint tmp = s.Joints[(JointType)int.Parse(jInfo[1])];
                    SkeletonPoint jointPos = new SkeletonPoint();
                    // Put the joint position data into the object
                    jointPos.X = float.Parse(jInfo[2]);
                    jointPos.Y = float.Parse(jInfo[3]);
                    jointPos.Z = float.Parse(jInfo[4]);
                    // Put the updated data back in the Skeleton's collection
                    tmp.Position = jointPos;
                    s.Joints[(JointType)int.Parse(jInfo[1])] = tmp;
                }
                if (skeletonOkay)
                {
                    skeletons.Add(s);
                }
            }

            return skeletons;
        }

        public static void writeToFile(List<Skeleton> skeletons, string filename)
        {
            // Keep track of which skeleton frame we're writing
            int frame = 0;
            // Open file for writing
            StreamWriter outputFile = new StreamWriter(filename);
            // Write each skeleton to file
            foreach (Skeleton s in skeletons)
            {
                foreach (Joint j in s.Joints)
                {
                    SkeletonPoint p = j.Position;
                    outputFile.WriteLine(frame + " " + (int)j.JointType + " " + p.X + " " + p.Y + " " + p.Z);
                }
            }
            // Close file to flush buffer
            outputFile.Close();
        }
    }
}
