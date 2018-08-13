using System;
using System.Collections.Generic;

namespace CardinalInventoryApp.Services.Interfaces
{
    public interface IWatchSessionManager
    {
        event EventHandler<WatchDataEventArgs> DataReceived;
        bool IsPairedSession();
        bool IsReachableSession();
        void StartSession();
        void StopSession();
        void SendData(WatchDataType type, string data);
        void SendData(WatchDataType type, double x, double y, double z);
    }

    public enum WatchDataType
    {
        GyroData,
        AccelData,
        DeviceMotionRotationRateData,
        DeviceMotionAttitudeData,
        DeviveMotionAccelData,
        WristLocationData
    };

    public class WatchDataEventArgs : EventArgs
    {
        public WatchDataType WatchDataType { get; set; }
        public string Data { get; set; }

        public WatchDataEventArgs(WatchDataType wdt, string data)
        {
            Data = data;
            WatchDataType = wdt;
        }

        public WatchDataEventArgs(string wdt, string data)
        {
            Data = data;
            if (wdt.Equals(WatchDataType.GyroData.ToString()))
            {
                WatchDataType = WatchDataType.GyroData;
            }
            else if (wdt.Equals(WatchDataType.AccelData.ToString()))
            {
                WatchDataType = WatchDataType.AccelData;
            }
            else if (wdt.Equals(WatchDataType.DeviceMotionRotationRateData.ToString()))
            {
                WatchDataType = WatchDataType.DeviceMotionRotationRateData;
            }
            else if (wdt.Equals(WatchDataType.DeviceMotionAttitudeData.ToString()))
            {
                WatchDataType = WatchDataType.DeviceMotionAttitudeData;
            }
            else if (wdt.Equals(WatchDataType.DeviveMotionAccelData.ToString()))
            {
                WatchDataType = WatchDataType.DeviveMotionAccelData;
            }
            else if(wdt.Equals(WatchDataType.WristLocationData.ToString()))
            {
                WatchDataType = WatchDataType.WristLocationData;
            }
        }
    }
}