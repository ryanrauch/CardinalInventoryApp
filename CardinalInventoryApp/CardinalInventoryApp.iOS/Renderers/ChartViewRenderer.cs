using System;
using CardinalInventoryApp.iOS.Renderers;
using CardinalInventoryApp.Views.ContentPages;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(ChartViewBase), typeof(ChartViewRenderer))]

namespace CardinalInventoryApp.iOS.Renderers
{
    public class ChartViewRenderer
    {
        public ChartViewRenderer()
        {
        }
    }
}
