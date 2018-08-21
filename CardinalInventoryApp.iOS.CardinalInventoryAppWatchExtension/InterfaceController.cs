using System;
using WatchKit;
using Foundation;
using CoreMotion;
using CardinalInventoryApp.Contracts;

namespace CardinalInventoryApp.iOS.CardinalInventoryAppWatchExtension
{
    public partial class InterfaceController : WKInterfaceController
    {
        const bool INCLUDEGYRO = false;
        const bool INCLUDEACCELEROMETER = false;

        const double _updateInterval = 1.0d / 60.0d; //0.10d;
        WCSessionManager _sessionManager;
        CMMotionManager _motionManager;

        private bool _isPouring;
        private ulong _pourStart;
        const double PITCHSTART = 0.261d;
        const double PITCHSTOP = 0.0d;

        protected InterfaceController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            _sessionManager = new WCSessionManager();
            _motionManager = new CMMotionManager
            {
                GyroUpdateInterval = _updateInterval,
                AccelerometerUpdateInterval = _updateInterval,
                DeviceMotionUpdateInterval = _updateInterval
            };
            _isPouring = false;
            _pourStart = 0;
        }

        public override void Awake(NSObject context)
        {
            base.Awake(context);

            // Configure interface objects here.
            Console.WriteLine("{0} awake with context", this);
            _sessionManager.StartSession();
        }

        public override void WillActivate()
        {
            // This method is called when the watch view controller is about to be visible to the user.
            Console.WriteLine("{0} will activate", this);

            WKExtension.SharedExtension.Autorotating = true;

            string initData = string.Format("{0}:{1}",
                                            _updateInterval,
                                            WKInterfaceDevice.CurrentDevice.WristLocation.ToString());
            myLabel.SetText(initData);
            _sessionManager.SendData(WatchDataType.InitializationData, initData);
            if (INCLUDEACCELEROMETER && _motionManager.AccelerometerAvailable)
            {
                Console.WriteLine("AccelerometerAvailable");
                _motionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
                {
                    _sessionManager.SendData(WatchDataType.AccelData, data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);
                });
            }
            if (INCLUDEGYRO && _motionManager.GyroAvailable)
            {
                Console.WriteLine("GyroAvailable");
                _motionManager.StartGyroUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
                {
                    _sessionManager.SendData(WatchDataType.GyroData, data.RotationRate.x, data.RotationRate.y, data.RotationRate.z);
                });
            }
            if (_motionManager.DeviceMotionAvailable)
            {
                Console.WriteLine("DeviceMotionAvailable");
                _motionManager.StartDeviceMotionUpdates(CMAttitudeReferenceFrame.XArbitraryZVertical, NSOperationQueue.CurrentQueue, (data, error) =>
                {
                    ////_sessionManager.SendData(WatchDataType.DeviceMotionRotationRateData, data.RotationRate.x, data.RotationRate.y, data.RotationRate.z);
                    //_sessionManager.SendData(WatchDataType.DeviceMotionAttitudeData, data.Attitude.Pitch, data.Attitude.Roll, data.Attitude.Yaw);
                    //_sessionManager.SendData(WatchDataType.DeviceMotionAccelData, data.UserAcceleration.X, data.UserAcceleration.Y, data.UserAcceleration.Z);
                    ////myLabel.SetText(string.Format("X{0} Y{1} Z{2}", data.UserAcceleration.X, data.UserAcceleration.Y, data.UserAcceleration.Z));
                    ////myLabel.SetText(string.Format("{0}", DateTime.Now.Ticks));
                    SmartWatchSessionData swsd = new SmartWatchSessionData()
                    {
                        AttitudePitch = data.Attitude.Pitch,
                        AttitudeRoll = data.Attitude.Roll,
                        AttitudeYaw = data.Attitude.Yaw,
                        UserAccelerationX = data.UserAcceleration.X,
                        UserAccelerationY = data.UserAcceleration.Y,
                        UserAccelerationZ = data.UserAcceleration.Z,
                        RotationRateX = data.RotationRate.x,
                        RotationRateY = data.RotationRate.y,
                        RotationRateZ = data.RotationRate.z,
                        TimestampUnixMs = Convert.ToUInt64(new DateTimeOffset(DateTime.Now.ToUniversalTime()).ToUnixTimeMilliseconds())
                    };
                    _sessionManager.SendData(swsd);
                    CheckPouring(swsd);
                });
            }
        }

        private void CheckPouring(SmartWatchSessionData data)
        {
            if(!_isPouring
               && data.AttitudePitch > PITCHSTART)
            {
                _isPouring = true;
                _pourStart = data.TimestampUnixMs;
                WKInterfaceDevice.CurrentDevice.PlayHaptic(WKHapticType.Start);
            }
            else if(_isPouring
                    && data.AttitudePitch < PITCHSTOP)
            {
                _isPouring = false;
                WKInterfaceDevice.CurrentDevice.PlayHaptic(WKHapticType.Stop);
            }
            else if(_isPouring
                    && data.TimestampUnixMs - _pourStart > 2000)
            {
                _isPouring = false;
                WKInterfaceDevice.CurrentDevice.PlayHaptic(WKHapticType.Stop);
            }
        }

        public override void DidDeactivate()
        {
            // This method is called when the watch view controller is no longer visible to the user.
            Console.WriteLine("{0} did deactivate", this);
            if(INCLUDEACCELEROMETER && _motionManager.AccelerometerActive)
            {
                _motionManager.StopAccelerometerUpdates();
            }
            if(INCLUDEGYRO && _motionManager.GyroActive)
            {
                _motionManager.StopGyroUpdates();
            }
            if(_motionManager.DeviceMotionActive)
            {
                _motionManager.StopDeviceMotionUpdates();
            }
            //_sessionManager.StopSession();
            WKExtension.SharedExtension.Autorotating = false;
        }
    }
}
