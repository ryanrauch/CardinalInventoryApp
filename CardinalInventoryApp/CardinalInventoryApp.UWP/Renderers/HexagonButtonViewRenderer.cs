using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using CardinalInventoryApp.UWP.Renderers;
using CardinalInventoryApp.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(HexagonButtonView), typeof(HexagonButtonViewRenderer))]
namespace CardinalInventoryApp.UWP.Renderers
{
    public class HexagonButtonViewRenderer : ViewRenderer<HexagonButtonView, Windows.UI.Xaml.Controls.Grid>
    {
        public HexagonButtonViewRenderer()
        {
            SizeChanged += HexagonButtonViewRenderer_SizeChanged;
        }

        private void HexagonButtonViewRenderer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawHexagon(e.NewSize);
        }

        private void DrawHexagon(Windows.Foundation.Size ns)
        {
            if (Element == null)
            {
                return;
            }
            //if(ns == Windows.Foundation.Size.Empty && Control != null)
            //{
            //    ns = new Windows.Foundation.Size(Control.Width, Control.Height);
            //}

            var gr = new Windows.UI.Xaml.Controls.Grid();
            LabelRenderer lr = new LabelRenderer();
            var poly = new Polygon();
            poly.StrokeThickness = Element.BorderSize;
            poly.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb((byte)(Element.BorderColor.A * 255),
                                                                      (byte)(Element.BorderColor.R * 255),
                                                                      (byte)(Element.BorderColor.G * 255),
                                                                      (byte)(Element.BorderColor.B * 255)));
            poly.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb((byte)(Element.BackgroundColor.A * 255),
                                                                      (byte)(Element.BackgroundColor.R * 255),
                                                                      (byte)(Element.BackgroundColor.G * 255),
                                                                      (byte)(Element.BackgroundColor.B * 255)));
            poly.Points = new PointCollection();
            double cx = ns.Width / 2;
            double cy = ns.Height / 2;
            for (int i = 0; i < 6; ++i)
            {
                poly.Points.Add(new Windows.Foundation.Point(cx + Element.Radius * Math.Cos((i * 60 - 30) * Math.PI / 180f),
                                                             cy + Element.Radius * Math.Sin((i * 60 - 30) * Math.PI / 180f)));
            }
            poly.FillRule = FillRule.EvenOdd;
            gr.Children.Add(poly);

            TextBlock tb = new TextBlock();
            tb.HorizontalTextAlignment = Windows.UI.Xaml.TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            if (!String.IsNullOrEmpty(Element.FAText))
            {
                tb.Text = Element.FAText;
                tb.FontFamily = new FontFamily(Element.FAFontFamily);
                tb.FontSize = Element.FAFontSize;
            }
            else if(!String.IsNullOrEmpty(Element.Text))
            {
                tb.Text = Element.Text;
                //tb.FontFamily = new FontFamily(Element.FontFamily);
                tb.FontSize = Element.FontSize;
            }
            tb.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb((byte)(Element.TextColor.A * 255),
                                                                          (byte)(Element.TextColor.R * 255),
                                                                          (byte)(Element.TextColor.G * 255),
                                                                          (byte)(Element.TextColor.B * 255)));
            gr.Children.Add(tb);
            gr.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            SetNativeControl(gr);
        }
    }
}
