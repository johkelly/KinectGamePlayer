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
        private int VotingWindowSize = 15;
        public SVMKeyboardTriggerer(HistogramSharePoint hsp)
        {
            this.hsp = hsp;

        }

        /// <summary>
        /// Taken from http://stackoverflow.com/a/20493025
        /// </summary>
        /// <param name="bVk"></param>
        /// <param name="bScan"></param>
        /// <param name="dwFlags"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        /// <summary>
        /// Declaration of external SendInput method, taken from http://stackoverflow.com/a/20493025
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput(
            uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize);

        /// <summary>
        /// Send a key down and up event pair via the Windows Input API, adapted from http://stackoverflow.com/a/20493025
        /// </summary>
        /// <param name="code"></param>
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

        /// <summary>
        /// Function which will run forever, continously classifying histogram batches into key events.
        /// </summary>
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
            SVMModel model = SVM.Train(problem, parameter);

            string[] eventTrigger = { "standing", "leftShoulder", "rightShoulder", "leftHip", "rightHip" };
            ScanCodeShort[] keyEvents = {ScanCodeShort.KEY_S, ScanCodeShort.KEY_I, ScanCodeShort.KEY_O, ScanCodeShort.KEY_K, ScanCodeShort.KEY_L};

            // Continuously scan the histogram share point for new histogram data
            while(true)
            {
                hb = hsp.histBatch;
                // Compare references -- if the share point has a different reference, we're out of date
                if (temp != hb && hb != null){
                    temp = hb;
                    int count = 1;
                    // Convert histogram bins into SVM feature vectors
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
                    // Get a prediction
                    double y = SVM.Predict(model, nodes.ToArray());
                    // Use a sliding of votes to filter out brief moments of misclassification
                    votingWindow.Add(y);
                    while (votingWindow.Count > VotingWindowSize)
                    {
                        votingWindow.RemoveAt(0);
                    }
                    // Neat one-liner taken from http://stackoverflow.com/a/8260598
                    // Group the votes, sorty by group size, select the largest, select the associated vote value
                    double vote = votingWindow.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;

                    // Change the console title to make it clear what the classifier is seeing
                    System.Console.Title = eventTrigger[(int)vote];

                    // Only trigger a keypress when the voted value changes
                    // This has the result of holding a pose being equivalent to quickly dropping it
                    // i.e., the gesture is invariant to duration
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
