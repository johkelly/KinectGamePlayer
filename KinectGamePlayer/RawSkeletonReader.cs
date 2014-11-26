using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3
{
    /// <summary>
    /// Intercepts Kinect data to process a steady stream of skeleton data
    /// </summary>
    class RawSkeletonReader
    {
        private KinectSensor sensor;

        public event EventHandler<Skeleton> rawSkeletonReady;
        private int currentPlayerId = -1;

        public RawSkeletonReader()
        {
            // Attempt to get the active Kinect
            foreach (var potentialSensor in KinectSensor.KinectSensors) {
                if (potentialSensor.Status == KinectStatus.Connected) {
                    sensor = potentialSensor;
                }
            }
            // Kinect was acquired, set up for incoming events
            if (null != sensor)
            {
                sensor.SkeletonStream.Enable();
                sensor.SkeletonFrameReady += handleSkeletonFrame;
                sensor.Start();
            }
        }

        /// <summary>
        /// Safely open the SkeletonFrame and pass it to any consumer listening for new skeleton data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleSkeletonFrame(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (rawSkeletonReady != null)
            {
                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame == null)
                    {
                        return;
                    }
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    try
                    {
                        frame.CopySkeletonDataTo(skeletons);
                        // Find the skeleton of the player being tracked
                        foreach (Skeleton s in skeletons)
                        {
                            if (currentPlayerId == -1 && s.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                currentPlayerId = s.TrackingId;
                            }
                            if (s.TrackingState == SkeletonTrackingState.Tracked &&  s.TrackingId == currentPlayerId)
                            {
                                // Inform any listeners a new skeleton is ready
                                rawSkeletonReady(this, s);
                                return;
                            }
                        }
                        // Did not see the current player ... drop tracking id
                        currentPlayerId = -1;
                    }
                    catch (InvalidOperationException)
                    {
                        // SkeletonFrame may throw when Kinect is in bad state
                        // Ignore frame in that case
                    }
                }
            }
        }
    }
}
