using System;
using CardinalInventoryApp.iOS.Renderers;
using CardinalInventoryApp.Views.ContentPages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ChartViewBase), typeof(ChartViewBaseRenderer))]

namespace CardinalInventoryApp.iOS.Renderers
{
    public class ChartViewBaseRenderer : PageRenderer
    {

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
        }
    }
}
