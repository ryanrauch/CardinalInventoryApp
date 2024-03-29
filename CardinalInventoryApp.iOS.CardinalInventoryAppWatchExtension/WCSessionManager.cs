﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using WatchConnectivity;

namespace CardinalInventoryApp.iOS.CardinalInventoryAppWatchExtension
{
    public class SmartWatchSessionData
    {
        public int Interval { get; set; }
        public Guid SmartWatchSessionId { get; set; }
        //public SmartWatchSession SmartWatchSession { get; set; }
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

    public enum WatchDataType
    {
        GyroData,
        AccelData,
        DeviceMotionRotationRateData,
        DeviceMotionAttitudeData,
        DeviceMotionAccelData,
        InitializationData,
        SmartWatchSessionDataObj
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
            else if (wdt.Equals(WatchDataType.DeviceMotionAccelData.ToString()))
            {
                WatchDataType = WatchDataType.DeviceMotionAccelData;
            }
            else if (wdt.Equals(WatchDataType.InitializationData.ToString()))
            {
                WatchDataType = WatchDataType.InitializationData;
            }
            else if (wdt.Equals(WatchDataType.SmartWatchSessionDataObj.ToString()))
            {
                WatchDataType = WatchDataType.SmartWatchSessionDataObj;
            }
        }
    }

    public class WCSessionManager : WCSessionDelegate
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public event EventHandler<WatchDataEventArgs> DataReceived;

        public WCSessionManager() : base()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
        }

        public bool IsPairedSession()
        {
#if __IOS__
            return _session.Paired;
#else
            return _validSession != null;
#endif
        }

        public bool IsReachableSession()
        {
            return _session.Reachable ? (_validSession != null) : false;
        }

        public void StartSession()
        {
            StartWCSession();
        }

        public void StopSession()
        {
            _session = null;
        }

        private long DateTimeToUnixMilliseconds(DateTime dt)
        {
            //return dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return new DateTimeOffset(dt.ToUniversalTime()).ToUnixTimeMilliseconds();
        }

        public void SendData(SmartWatchSessionData data)
        {
            string serialized = JsonConvert.SerializeObject(data, _serializerSettings);
            SendData(WatchDataType.SmartWatchSessionDataObj, serialized);
        }

        public void SendData(WatchDataType type, double x, double y, double z)
        {
            //string data = string.Format("{0:0.000}:{1:0.000}:{2:0.000}:{3}", x, y, z, DateTimeToUnixMilliseconds(DateTime.Now));
            string data = string.Format("{0:0.##}:{1:0.##}:{2:0.##}:{3}", x, y, z, DateTimeToUnixMilliseconds(DateTime.Now));
            SendData(type, data);
        }

        public void SendData(WatchDataType type, string data)
        {
            Console.WriteLine(type.ToString() + "-" + data);
            var context = new Dictionary<string, object>
            {
                { type.ToString(), data }
            };
            UpdateApplicationContext(context);
        }


        // Setup is converted from https:-//www.natashatherobot.com/watchconnectivity-say-hello-to-wcsession/ 
        // below code is taken from https:-//github.com/xamarin/ios-samples/blob/70d3660d8e184d438cf1df52ab8ce3ee70d261b8/watchOS/WatchConnectivity/WatchConnectivity.OnWatchExtension/SessionManager/WCSessionManager.cs

        private static readonly WCSessionManager _sharedManager = new WCSessionManager();
        private static WCSession _session = WCSession.IsSupported ? WCSession.DefaultSession : null;

#if __IOS__
        public static string Device = "Phone";
#else
        public static string Device = "Watch";
#endif

        private WCSession _validSession
        {
            get
            {
#if __IOS__
                Console.WriteLine($"Paired status:{(_session.Paired ? 'Y' : 'N')}\n");
                Console.WriteLine($"Watch App Installed status:{(_session.WatchAppInstalled ? 'Y' : 'N')}\n");
                return (_session.Paired && _session.WatchAppInstalled) ? _session : null;
#else
                return _session;
#endif
            }
        }

        public static WCSessionManager SharedManager
        {
            get
            {
                return _sharedManager;
            }
        }

        public void StartWCSession()
        {
            if (_session != null)
            {
                _session.Delegate = this;
                _session.ActivateSession();
                Console.WriteLine($"Started Watch Connectivity Session on {Device}");
            }
        }

        public override void SessionReachabilityDidChange(WCSession session)
        {
            Console.WriteLine($"Watch connectivity Reachable:{(session.Reachable ? 'Y' : 'N')} from {Device}");
            // handle session reachability change
            if (session.Reachable)
            {
                // great! continue on with Interactive Messaging
            }
            else
            {
                // prompt the user to unlock their iOS device
            }
        }

        #region Application Context Methods

        private void UpdateApplicationContext(Dictionary<string, object> applicationContext)
        {
            // Application context doesnt need the watch to be reachable, it will be received when opened
            if (_validSession != null)
            {
                try
                {
                    var NSValues = applicationContext.Values.Select(x => new NSString(JsonConvert.SerializeObject(x))).ToArray();
                    var NSKeys = applicationContext.Keys.Select(x => new NSString(x)).ToArray();
                    var NSApplicationContext = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(NSValues, NSKeys, NSValues.Count());

                    var sendSuccessfully = _validSession.UpdateApplicationContext(NSApplicationContext, out NSError error);
                    if (sendSuccessfully)
                    {
                        Console.WriteLine($"Sent App Context from {Device} \nPayLoad: {NSApplicationContext.ToString()} \n");
                    }
                    else
                    {
                        Console.WriteLine($"Error Updating Application Context: {error.LocalizedDescription}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Updating Application Context: {ex.Message}");
                }
            }
        }

        public override void DidReceiveApplicationContext(WCSession session, NSDictionary<NSString, NSObject> applicationContext)
        {
            Console.WriteLine($"Receiving Message on {Device}");
            if (DataReceived != null)
            {
                var keys = applicationContext.Keys.Select(k => k.ToString()).ToArray();
                var values = applicationContext.Values.Select(v => JsonConvert.DeserializeObject(v.ToString())).ToArray();
                var dictionary = keys.Zip(values, (k, v) => new { Key = k, Value = v })
                                     .ToDictionary(x => x.Key, x => x.Value);

                foreach (var k in keys)
                {
                    DataReceived(this, new WatchDataEventArgs(k, dictionary[k].ToString()));
                }
            }
        }
        #endregion
    }
}
