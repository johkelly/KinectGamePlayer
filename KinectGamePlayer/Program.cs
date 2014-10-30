using LibSVMsharp;
using LibSVMsharp.Helpers;
using MathNet.Numerics.Statistics;
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
            RawSkeletonReader reader = new RawSkeletonReader();
            SkeletonFrameWindowProcessor proc = new SkeletonFrameWindowProcessor(hsp);
            reader.rawSkeletonReady += proc.handleNewSkeleton;
            Thread t = new Thread(trigger.run);
            t.Start();
            Random r = new Random();
            while (true)
            {
            }
        }
    }
}
