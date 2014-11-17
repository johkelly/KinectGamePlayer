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
            parameter.C = 9.84915530676;
            parameter.Gamma = 0.0078125;
            string[] eventTrigger = { "standing", "leftShoulder", "rightShoulder", "leftHip", "rightHip" };
            //char[] eventTrigger = {'s', 'l', 'L', 's', 'S', 'h', 'H', 'f', 'b'};
            
            SVMModel model = SVM.Train(problem, parameter);
            while(true)
            {
                hb = hsp.histBatch;
                if (temp != hb){
                    temp = hb;
                    int count = 1;
                    List<SVMNode> nodes = new List<SVMNode>();
                    for (int i = 0; i < temp.Count; i++)
                    {
                        Histogram histObject = temp[i];
                        for (int j = 0; j < histObject.BucketCount; j++)
                        {
                            SVMNode node = new SVMNode();
                            node.Index = count++;
                            node.Value = histObject[j].Count/SkeletonFrameWindowProcessor.WINDOW_SIZE;
                            nodes.Add(node);
                        }
                    }
                    double y = SVM.Predict(model, nodes.ToArray());
                    System.Console.WriteLine("" + eventTrigger[(int)y]);
                    //for (int i = 0; i < nodes.Count; ++i)
                    //{
                    //    System.Console.Write(i + ":" + nodes[i].Value + " ");
                    //}
                    //System.Console.WriteLine("");
                    if (true)
                    {
                        //System.Windows.Forms.SendKeys.SendWait("" + eventTrigger[(int)y]);
                    }

                    


                }
            }

        }
    }
}
