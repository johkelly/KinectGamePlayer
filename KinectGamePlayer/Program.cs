using CSCI598.Proj3.Histogrammer;
using HistogrammerRunner;
using LibSVMsharp;
using LibSVMsharp.Helpers;
using MathNet.Numerics.Statistics;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSCI598.Proj3
{
    class Program
    {
        static void Main(string[] args)
        {
            HistogramSharePoint hsp = new HistogramSharePoint();
            SVMKeyboardTriggerer trigger = new SVMKeyboardTriggerer(hsp);
            SkeletonFrameWindowProcessor proc = new SkeletonFrameWindowProcessor(hsp, makeRADHistogrammer(RADSkeletonHistogrammer.DefaultJointList, PipelineConstants.SVMBoundsFile));
            Thread t = new Thread(trigger.run);
            t.Start();
            while (true)
            {
            }
        }

        private static SkeletonHistogrammer makeHJPDHistogrammer(string datapath)
        {
            string[] binDefLines = System.IO.File.ReadAllLines(datapath);
            SortedDictionary<JointType, BinDefinition> binDefinitions = new SortedDictionary<JointType, BinDefinition>();
            foreach (string binDefLine in binDefLines)
            {
                string[] vals = binDefLine.Split();
                BinDefinition binDef = new BinDefinition();
                binDef.lowerBound = double.Parse(vals[1]);
                binDef.numBins = int.Parse(vals[2]);
                binDef.upperBound = double.Parse(vals[3]);
                binDefinitions[(JointType)int.Parse(vals[0])] = binDef;
            }
            return new HJPDSkeletonHistogrammer(binDefinitions);
        }

        private static SkeletonHistogrammer makeRADHistogrammer(List<JointType> jointList, string datapath)
        {
            string[] binDefLines = System.IO.File.ReadAllLines(datapath);
            List<BinDefinition> binDefinitions = new List<BinDefinition>();
            foreach (string binDefLine in binDefLines)
            {
                string[] vals = binDefLine.Split();
                BinDefinition binDef = new BinDefinition()
                {
                    lowerBound = double.Parse(vals[1]),
                    upperBound = double.Parse(vals[3]),
                    numBins = int.Parse(vals[2])
                };
                binDefinitions.Add(binDef);
            }
            return new RADSkeletonHistogrammer(jointList, binDefinitions);
        }
    }
}
