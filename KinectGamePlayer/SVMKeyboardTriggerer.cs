using HistogrammerRunner;
using LibSVMsharp;
using LibSVMsharp.Helpers;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSCI598.Proj3
{
    // Keypressing: http://stackoverflow.com/a/20493025
    class SVMKeyboardTriggerer
    {
        private HistogramSharePoint hsp;
        private List<double> votingWindow = new List<double>();
        private double previousVote;
        public SVMKeyboardTriggerer(HistogramSharePoint hsp)
        {
            this.hsp = hsp;

        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        /// <summary>
        /// Declaration of external SendInput method
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput(
            uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize);

        void SendInputWithAPI(ScanCodeShort code)
        {
            INPUT[] Inputs = new INPUT[2];

            INPUT Input = new INPUT();
            Input.type = 1; // 1 = Keyboard Input
            Input.U.ki.wScan = code;
            Input.U.ki.dwFlags = KEYEVENTF.SCANCODE;
            Inputs[0] = Input;

            INPUT Input2 = new INPUT();
            Input2.type = 1; // 1 = Keyboard Input
            Input2.U.ki.wScan = code;
            Input2.U.ki.dwFlags = KEYEVENTF.SCANCODE | KEYEVENTF.KEYUP;
            Inputs[1] = Input2;

            SendInput(2, Inputs, INPUT.Size);
        }

        public void run()
        {
            List<Histogram> temp = null;
            List<Histogram> hb;
            SVMProblem problem = SVMProblemHelper.Load(PipelineConstants.SVMFeaturesFile);

            SVMParameter parameter = new SVMParameter();
            parameter.Type = SVMType.C_SVC;
            parameter.Kernel = SVMKernelType.RBF;
            parameter.C = 13.9;
            parameter.Gamma = .029;
            string[] eventTrigger = { "standing", "leftShoulder", "rightShoulder", "leftHip", "rightHip" };
            
            ScanCodeShort[] keyEvents = {ScanCodeShort.KEY_S, ScanCodeShort.KEY_I, ScanCodeShort.KEY_O, ScanCodeShort.KEY_K, ScanCodeShort.KEY_L};

            SVMModel model = SVM.Train(problem, parameter);
            while(true)
            {
                hb = hsp.histBatch;
                if (temp != hb && hb != null){
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
                            node.Value = histObject[j].Count/SkeletonFrameWindowProcessor.WindowSize;
                            nodes.Add(node);
                        }
                    }
                    double y = SVM.Predict(model, nodes.ToArray());
                    votingWindow.Add(y);
                    while (votingWindow.Count > 15)
                    {
                        votingWindow.RemoveAt(0);
                    }
                    // http://stackoverflow.com/a/8260598
                    double vote = votingWindow.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;
                    System.Console.Title = eventTrigger[(int)vote];

                    if (vote != 0 && vote != previousVote)
                    {
                        SendInputWithAPI(keyEvents[(int)vote]);
                    }
                    previousVote = vote;
                }
            }

        }
    }
}
