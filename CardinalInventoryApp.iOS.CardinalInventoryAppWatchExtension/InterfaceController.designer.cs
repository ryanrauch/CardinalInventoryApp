// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CardinalInventoryApp.iOS.CardinalInventoryAppWatchExtension
{
    [Register ("InterfaceController")]
    partial class InterfaceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel myLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (myLabel != null) {
                myLabel.Dispose ();
                myLabel = null;
            }
        }
    }
}