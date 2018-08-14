using System;
using System.Collections.Generic;
using System.Text;

namespace CardinalInventoryApp.Contracts
{
    public class SmartWatchSessionData
    {
        public int Interval { get; set; }
        public Guid SmartWatchSessionId { get; set; }
        public SmartWatchSession SmartWatchSession { get; set; }
        public double AttitudePitch { get; set; }
        public double AttitudeRoll { get; set; }
        public double AttitudeYaw { get; set; }
        public double RotationRateX { get; set; }
        public double RotationRateY { get; set; }
        public double RotationRateZ { get; set; }
        public double UserAccelerationX { get; set; }
        public double UserAccelerationY { get; set; }
        public double UserAccelerationZ { get; set; }
        public double AccelerometerX { get; set; }
        public double AccelerometerY { get; set; }
        public double AccelerometerZ { get; set; }
        public UInt64 TimestampUnixMs { get; set; }
    }
}
