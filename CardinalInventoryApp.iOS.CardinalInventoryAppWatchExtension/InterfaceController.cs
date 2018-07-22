using System;

using WatchKit;
using Foundation;
using CoreMotion;

namespace CardinalInventoryApp.iOS.CardinalInventoryAppWatchExtension
{
    public partial class InterfaceController : WKInterfaceController
    {
        private WCSessionManager _sessionManager;
        private CMMotionManager _motionManager;

        partial void OnButtonPress()
        {
            myLabel.SetText("clicked");
            //throw new NotImplementedException();
        }

        protected InterfaceController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void Awake(NSObject context)
        {
            base.Awake(context);

            // Configure interface objects here.
            Console.WriteLine("{0} awake with context", this);
            myLabel.SetText("label");
            _motionManager = new CMMotionManager();
            _sessionManager = new WCSessionManager();
            _sessionManager.StartSession();
            _motionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            {
                _sessionManager.SendData(WatchDataType.AccelDataX, data.Acceleration.X.ToString("0.00000000"));
                _sessionManager.SendData(WatchDataType.AccelDataY, data.Acceleration.Y.ToString("0.00000000"));
                _sessionManager.SendData(WatchDataType.AccelDataZ, data.Acceleration.Z.ToString("0.00000000"));
                //this.lblX.Text = data.Acceleration.X.ToString("0.00000000");
                //this.lblY.Text = data.Acceleration.Y.ToString("0.00000000");
                //this.lblZ.Text = data.Acceleration.Z.ToString("0.00000000");
                myLabel.SetText(data.Acceleration.X.ToString("0.00000000"));
            });
        }

        public override void WillActivate()
        {
            // This method is called when the watch view controller is about to be visible to the user.
            Console.WriteLine("{0} will activate", this);
        }

        public override void DidDeactivate()
        {
            // This method is called when the watch view controller is no longer visible to the user.
            Console.WriteLine("{0} did deactivate", this);
        }
    }
}
