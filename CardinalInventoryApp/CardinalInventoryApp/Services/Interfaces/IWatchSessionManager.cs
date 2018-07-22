using System;
using System.Collections.Generic;

namespace CardinalInventoryApp.Services.Interfaces
{
    public enum WatchDataType
    {
        GyroDataX,
        GyroDataY,
        GyroDataZ,
        AccelDataX,
        AccelDataY,
        AccelDataZ
    };

    public interface IWatchSessionManager
    {
        event EventHandler<WatchDataEventArgs> DataReceived;
        bool IsPairedSession();
        bool IsReachableSession();
        void StartSession();
        void SendData(WatchDataType type, string data);
    }

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
            if (wdt.Equals(WatchDataType.GyroDataX.ToString()))
            {
                WatchDataType = WatchDataType.GyroDataX;
            }
            else if (wdt.Equals(WatchDataType.GyroDataY.ToString()))
            {
                WatchDataType = WatchDataType.GyroDataY;
            }
            else if (wdt.Equals(WatchDataType.GyroDataZ.ToString()))
            {
                WatchDataType = WatchDataType.GyroDataZ;
            }
            else if (wdt.Equals(WatchDataType.AccelDataX.ToString()))
            {
                WatchDataType = WatchDataType.AccelDataX;
            }
            else if (wdt.Equals(WatchDataType.AccelDataY.ToString()))
            {
                WatchDataType = WatchDataType.AccelDataY;
            }
            else if (wdt.Equals(WatchDataType.AccelDataZ.ToString()))
            {
                WatchDataType = WatchDataType.AccelDataZ;
            }
        }
    }
}