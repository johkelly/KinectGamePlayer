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
            Thread t = new Thread(trigger.run);
            t.Start();
            Random r = new Random();
            while (true)
            {
                hsp.histBatch = new List<Histogram>();
                if (r.Next(10) < 5)
                {
                    hsp.histBatch = null;
                }
            }
        }
    }
}
