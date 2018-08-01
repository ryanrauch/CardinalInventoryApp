using CoreVideo;

namespace CardinalInventoryApp.iOS.ScanBarcode
{
    /// <summary>
    /// An object that receives frames for querying (i.e., `RectangleScanner`, `ObjectTracker`)
    /// </summary>
    interface IRectangleViewer
    {
        void OnFrameCaptured(CVPixelBuffer buffer);
    }
}