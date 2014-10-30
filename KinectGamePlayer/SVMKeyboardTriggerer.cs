using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSCI598.Proj3
{
    class SVMKeyboardTriggerer
    {
        private HistogramSharePoint hsp;
        public SVMKeyboardTriggerer(HistogramSharePoint hsp)
        {
            this.hsp = hsp;
        }

        public void run()
        {
            while (true)
            {
                var hb = hsp.histBatch;
                System.Console.WriteLine((hb == null ? "null" : hb.ToString()));
                Thread.Sleep(500);
            }
        }
    }
}
