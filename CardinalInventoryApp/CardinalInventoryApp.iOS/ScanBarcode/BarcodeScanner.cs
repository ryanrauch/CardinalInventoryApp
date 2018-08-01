using System;
using System.Linq;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using UIKit;
using Vision;

namespace CardinalInventoryApp.iOS.ScanBarcode
{
    /// <summary>
	/// Makes Vision requests in "scanning" mode -- looks for rectangles
	/// </summary>
	internal class BarcodeScanner : NSObject, IRectangleViewer
    {
        /// <summary>
        /// Connection to the Vision subsystem
        /// </summary>
        VNDetectBarcodesRequest barcodeRequest;

        /// <summary>
        /// The set of detected barcodes
        /// </summary> 
        VNBarcodeObservation[] observations;

        /// <summary>
        /// Display overlay
        /// </summary>
        Overlay overlay;

        internal BarcodeScanner(Overlay overlay)
        {
            this.overlay = overlay;

            barcodeRequest = new VNDetectBarcodesRequest(BarcodesDetected);
            barcodeRequest.Symbologies = new VNBarcodeSymbology[] 
            {
                VNBarcodeSymbology.Ean13,   //UPC/Product Labels
                VNBarcodeSymbology.Code128  //TABC Labels
            };
        }

        /// <summary>
        /// Called by `ViewController.OnFrameCaptured` once per frame with the buffer processed by the image-processing pipeline in 
        /// `VideoCaptureDelegate.DidOutputSampleBuffer`
        /// </summary>
        /// <param name="buffer">The captured video frame.</param>
        public void OnFrameCaptured(CVPixelBuffer buffer)
        {

            //BeginInvokeOnMainThread(() => overlay.Message = $"Scanning Barcodes...");

            // Run the rectangle detector
            var handler = new VNImageRequestHandler(buffer, new NSDictionary());
            NSError error;
            handler.Perform(new VNRequest[] { barcodeRequest }, out error);
            if (error != null)
            {
                Console.Error.WriteLine(error);
                BeginInvokeOnMainThread(() => overlay.Message = error.ToString());
            }
        }

        /// <summary>
        /// Asynchronously called by the Vision subsystem subsequent to `Perform` in `OnFrameCaptured` 
        /// </summary>
        /// <param name="request">The request sent to the Vision subsystem.</param>
        /// <param name="err">If not null, describes an error in Vision.</param>
        private void BarcodesDetected(VNRequest request, NSError err)
        {
            if (err != null)
            {
                overlay.Message = err.ToString();
                Console.Error.WriteLine(err);
                return;
            }
            overlay.Clear();

            observations = request.GetResults<VNBarcodeObservation>();
            overlay.StrokeColor = UIColor.Blue.CGColor;

            //Draw all detected rectangles in blue
            foreach (var o in observations)
            {
                var quad = new[] { o.TopLeft, o.TopRight, o.BottomRight, o.BottomLeft };
                BarcodeDetected(quad, o.Symbology, o.PayloadStringValue);
            }
        }

        private void BarcodeDetected(CGPoint[] normalizedQuadrilateral, VNBarcodeSymbology symbology, string code)
        {
            overlay.InvokeOnMainThread(() =>
            {
                // Note conversion from inverted coordinate system!
                var rotatedQuadrilateral = normalizedQuadrilateral.Select(pt => new CGPoint(pt.X, 1.0 - pt.Y)).ToArray();
                overlay.AddQuad(rotatedQuadrilateral);

                overlay.Message = observations.Count().ToString() + ":" + symbology.ToString() + ":" + code;
            });
        }


        private static bool ObservationContainsPoint(VNBarcodeObservation o, CGPoint normalizedPoint)
        {
            // Enhancement: This is actually wrong, since the touch could be within the bounding box but outside the quadrilateral. 
            // For better accuracy, implement the Winding Rule algorithm 
            return o.BoundingBox.Contains(normalizedPoint);
        }

        internal VNBarcodeObservation Containing(CGPoint normalizedPoint) => observations.FirstOrDefault(o => ObservationContainsPoint(o, normalizedPoint));
    }
}