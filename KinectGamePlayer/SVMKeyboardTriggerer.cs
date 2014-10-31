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
    class SVMKeyboardTriggerer
    {
        private HistogramSharePoint hsp;
        public SVMKeyboardTriggerer(HistogramSharePoint hsp)
        {
            this.hsp = hsp;

        }

        public void run()
        {
            List<Histogram> temp = null;
            List<Histogram> hb;
            //while (true)
            //{
            //   hb = hsp.histBatch;
            //    System.Console.WriteLine((hb == null ? "null" : hb.ToString()));
            //    Thread.Sleep(500);
            //}
            SVMProblem problem = SVMProblemHelper.Load(@"dataset_path.txt");

            SVMParameter parameter = new SVMParameter();
            parameter.Type = SVMType.C_SVC;
            parameter.Kernel = SVMKernelType.RBF;
            parameter.C = 1;
            parameter.Gamma = 1;
            char[] eventTrigger = {'0', 'x', 'y', 'z', 'c', 'v'};
            
            SVMModel model = SVM.Train(problem, parameter);
            while(true)
            {
                hb = hsp.histBatch;
                if (temp != hb){
                    temp = hb;
                    int count = 1;
                    List<SVMNode> nodes = new List<SVMNode>();
                    for (int i = 1; i < temp.Count; i++)
                    {
                        Histogram histObject = temp[i];
                        for (int j = 1; j < histObject.BucketCount; j++)
                        {
                            SVMNode node = new SVMNode();
                            node.Index = count++;
                            node.Value = histObject[j].Count;
                            nodes.Add(node);
                        }
                    }
                    double y = SVM.Predict(model, nodes.ToArray());
                    System.Console.WriteLine("" + eventTrigger[(int)y]);
                    if (y!=0)
                    {
                      //  System.Windows.Forms.SendKeys.Send(""+eventTrigger[(int)y]);
                    }

                    


                }
            }

        }
    }
}
