using System;

using WatchKit;
using Foundation;
using CoreMotion;

namespace CardinalInventoryApp.iOS.CardinalInventoryAppWatchExtension
{
    public partial class InterfaceController : WKInterfaceController
    {
        const double _updateInterval = 0.25d;
        WCSessionManager _sessionManager;
        CMMotionManager _motionManager;

        partial void OnButtonPress()
        {
            WKExtension.SharedExtension.Autorotating = !WKExtension.SharedExtension.Autorotating;
        }

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
            if (_motionManager.AccelerometerAvailable)
            {
                Console.WriteLine("AccelerometerAvailable");
                _motionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
                {
                    _sessionManager.SendData(WatchDataType.AccelData, data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);
                });
            }
            if (_motionManager.GyroAvailable)
            {
                Console.WriteLine("GyroAvailable");
                _motionManager.StartGyroUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
                {
                    _sessionManager.SendData(WatchDataType.GyroData, data.RotationRate.x, data.RotationRate.y, data.RotationRate.z);
                });
            }
            if(_motionManager.DeviceMotionAvailable)
            {
                Console.WriteLine("DeviceMotionAvailable");
                _motionManager.StartDeviceMotionUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
                {
                    _sessionManager.SendData(WatchDataType.DeviceMotionRotationRateData, data.RotationRate.x, data.RotationRate.y, data.RotationRate.z);
                    _sessionManager.SendData(WatchDataType.DeviceMotionAttitudeData, data.Attitude.Pitch, data.Attitude.Roll, data.Attitude.Yaw);
                    _sessionManager.SendData(WatchDataType.DeviveMotionAccelData, data.UserAcceleration.X, data.UserAcceleration.Y, data.UserAcceleration.Z);
                    myLabel.SetText(string.Format("X{0} Y{1} Z{2}", data.UserAcceleration.X, data.UserAcceleration.Y, data.UserAcceleration.Z));
                });
            }
        }

        public override void DidDeactivate()
        {
            // This method is called when the watch view controller is no longer visible to the user.
            Console.WriteLine("{0} did deactivate", this);
            if(_motionManager.AccelerometerActive)
            {
                _motionManager.StopAccelerometerUpdates();
            }
            if(_motionManager.GyroActive)
            {
                _motionManager.StopGyroUpdates();
            }
            if(_motionManager.DeviceMotionActive)
            {
                _motionManager.StopDeviceMotionUpdates();
            }
        }
    }
}
