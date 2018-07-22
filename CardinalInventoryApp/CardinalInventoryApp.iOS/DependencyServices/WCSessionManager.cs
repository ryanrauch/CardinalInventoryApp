
using System;
using System.Collections.Generic;
using System.Linq;
using CardinalInventoryApp.Services.Interfaces;
using Foundation;
using Newtonsoft.Json;
using WatchConnectivity;

namespace CardinalInventoryApp.iOS.DependencyServices
{
    public sealed class WCSessionManager : WCSessionDelegate, IWatchSessionManager<string>
    {
        public event EventHandler<string> MessageReceived;

        bool IsPairedSession()
        {
            return _session.Paired;
        }

        public bool IsReachableSession()
        {
            return _session.Reachable ? (_validSession != null) : false;
        }

        public void StartSession()
        {
            StartWCSession();
        }

        public void SendMessage(string msg)
        {

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

        public event ApplicationContextUpdatedHandler ApplicationContextUpdated;
        public delegate void ApplicationContextUpdatedHandler(WCSession session, Dictionary<string, object> applicationContext);

        private WCSession _validSession
        {
            get
            {
#if __IOS__
                Console.WriteLine($"Paired status:{(_session.Paired ? '✓' : '✗')}\n");
                Console.WriteLine($"Watch App Installed status:{(_session.WatchAppInstalled ? '✓' : '✗')}\n");
                return (_session.Paired && _session.WatchAppInstalled) ? _session : null;
#else
                return session;
#endif
            }
        }

        private WCSessionManager() : base() { }

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
            Console.WriteLine($"Watch connectivity Reachable:{(session.Reachable ? '✓' : '✗')} from {Device}");
            // handle session reachability change
            if (session.Reachable)
            {
                // great! continue on with Interactive Messaging
            }
            else
            {
                // 😥 prompt the user to unlock their iOS device
            }
        }

        #region Application Context Methods

        public void UpdateApplicationContext(Dictionary<string, object> applicationContext)
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
            if (ApplicationContextUpdated != null)
            {
                var keys = applicationContext.Keys.Select(k => k.ToString()).ToArray();
                var values = applicationContext.Values.Select(v => JsonConvert.DeserializeObject(v.ToString())).ToArray();
                var dictionary = keys.Zip(values, (k, v) => new { Key = k, Value = v })
                                     .ToDictionary(x => x.Key, x => x.Value);

                ApplicationContextUpdated(session, dictionary);
            }
        }

        #endregion
    }
}
