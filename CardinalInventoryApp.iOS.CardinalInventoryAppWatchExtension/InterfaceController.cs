using System;

using WatchKit;
using Foundation;

namespace CardinalInventoryApp.iOS.CardinalInventoryAppWatchExtension
{
    public partial class InterfaceController : WKInterfaceController
    {
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
