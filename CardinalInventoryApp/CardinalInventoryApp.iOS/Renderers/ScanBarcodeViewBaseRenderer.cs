using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVFoundation;
using CardinalInventoryApp.iOS.Renderers;
using CardinalInventoryApp.iOS.ScanBarcode;
using CardinalInventoryApp.Views.ContentPages;
using CoreGraphics;
using CoreVideo;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(ScanBarcodeView), typeof(ScanBarcodeViewBaseRenderer))]
namespace CardinalInventoryApp.iOS.Renderers
{
    public class ScanBarcodeViewBaseRenderer : PageRenderer
    {
        VideoCapture captureController;
        VideoCaptureDelegate captureDelegate;
        //RectangleScanner scanner;
        BarcodeScanner barcodeScanner;
        ObjectTracker tracker;
        IRectangleViewer activeViewer;

        AVCaptureVideoPreviewLayer previewLayer;
        Overlay overlay;
        UIView topBlurView;
        UIView bottomBlurView;
        UIView previewView;
        UIImageView bufferImageView = new UIImageView();
        UIButton resetButton;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            previewView = new UIView();
            previewView.Frame = View.Bounds;
            View.AddSubview(previewView);

            ConfigureBlurViews(View);
            View.AddSubview(ConfigureResetButton());
            View.AddSubview(ConfigureOverlay(topBlurView, bottomBlurView));
            View.AddSubview(ConfigureBufferImageView());

            ConfigureInitialVisionTask();

            previewLayer = new AVCaptureVideoPreviewLayer(captureController.Session);
            previewView.Layer.AddSublayer(previewLayer);
        }

        private UIView ConfigureBufferImageView()
        {
            bufferImageView.Frame = new CGRect(10, 30, 108, 115);
            return bufferImageView;
        }

        private UIView ConfigureOverlay(UIView tbv, UIView bbv)
        {
            //Configure layer on which we do our graphics
            overlay = new Overlay
            {
                Frame = new CGRect(tbv.Frame.Left, tbv.Frame.Bottom + 5, tbv.Frame.Right, bbv.Frame.Top - tbv.Frame.Bottom - 10),
                BackgroundColor = UIColor.Clear
            };
            return overlay;
        }


        private UIView ConfigureResetButton()
        {
            resetButton = new UIButton();
            resetButton.SetTitle("Reset", UIControlState.Normal);
            resetButton.Hidden = true;
            resetButton.TouchDown += ResetTracking;
            resetButton.TranslatesAutoresizingMaskIntoConstraints = false;
            resetButton.Frame = new CGRect(View.Frame.Right - 180, 40, 150, 50);
            return resetButton;
        }

        void ConfigureInitialVisionTask()
        {
            // Assert overlay initialized
            barcodeScanner = new BarcodeScanner(overlay);
            //scanner = new RectangleScanner(overlay);
            tracker = new ObjectTracker(overlay);

            activeViewer = barcodeScanner;
            captureDelegate = new VideoCaptureDelegate(OnFrameCaptured);
            captureController = new VideoCapture(captureDelegate);
        }

        void ResetTracking(Object sender, EventArgs e)
        {
            overlay.Message = "Scanning Barcodes...";
            activeViewer = barcodeScanner;
            resetButton.Hidden = true;
        }

        private void ConfigureBlurViews(UIView mainView)
        {
            var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Regular);
            topBlurView = new UIVisualEffectView(blur);
            mainView.AddSubview(topBlurView);
            bottomBlurView = new UIVisualEffectView(blur);
            mainView.AddSubview(bottomBlurView);
        }

        /// <summary>
		/// Handles frame captured event: forwards frame to active viewer, displays frame
		/// </summary>
		/// <param name="sender">The `VideoCaptureDelegate` delegate-object</param>
		/// <param name="args">EventArgsT containing the (processed) frame</param>
		void OnFrameCaptured(Object sender, EventArgsT<CVPixelBuffer> args)
        {
            var buffer = args.Value;
            activeViewer.OnFrameCaptured(buffer);

            // Display it
            var img = VideoCapture.ImageBufferToUIImage(buffer);
            overlay.BeginInvokeOnMainThread(() =>
            {
                var oldImg = bufferImageView.Image;
                if (oldImg != null)
                {
                    oldImg.Dispose();
                }
                bufferImageView.Image = img;
                bufferImageView.SetNeedsDisplay();
            });
        }

        /// <summary>
		/// Sees if the touch is inside a detected rectangle. If so, switches to "Tracking" mode
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            var touch = touches.First() as UITouch;
            var pt = touch.LocationInView(overlay);
            var normalizedPoint = new CGPoint(pt.X / overlay.Frame.Width, pt.Y / overlay.Frame.Height);
            if (activeViewer == barcodeScanner)
            {
                var trackedRectangle = barcodeScanner.Containing(normalizedPoint);
                if (trackedRectangle != null)
                {
                    tracker.Track(trackedRectangle);
                    overlay.Message = "Target acquired";
                    activeViewer = tracker;
                    resetButton.Hidden = false;
                }
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            previewLayer.Frame = previewView.Bounds;

            var oneFifthHeight = previewLayer.Frame.Height / 5;
            topBlurView.Frame = new CGRect(previewLayer.Frame.Left, previewLayer.Frame.Top, previewLayer.Frame.Right, oneFifthHeight);
            bottomBlurView.Frame = new CGRect(previewLayer.Frame.Left, previewLayer.Frame.Bottom - oneFifthHeight, previewLayer.Frame.Right, oneFifthHeight);
            overlay.Frame = new CGRect(topBlurView.Frame.Left, topBlurView.Frame.Bottom, topBlurView.Frame.Right, bottomBlurView.Frame.Top - topBlurView.Frame.Bottom);
            resetButton.Frame = new CGRect(View.Frame.Right - 180, 40, 150, 50);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}