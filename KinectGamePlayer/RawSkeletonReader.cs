using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3
{
    class RawSkeletonReader
    {
        private KinectSensor sensor;

        public event EventHandler<Skeleton> rawSkeletonReady;
        private int currentPlayerId;

        public RawSkeletonReader()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors) {
                if (potentialSensor.Status == KinectStatus.Connected) {
                    sensor = potentialSensor;
                }
            }

            if (null != sensor)
            {
                sensor.SkeletonStream.Enable();
                sensor.SkeletonFrameReady += handleSkeletonFrame;
            }
        }

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
                            if (currentPlayerId == -1 && s.TrackingId != -1)
                            {
                                currentPlayerId = s.TrackingId;
                            }
                            if (s.TrackingId == currentPlayerId)
                            {
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
