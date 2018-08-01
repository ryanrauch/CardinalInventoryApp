using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CardinalInventoryApp.Controls
{
    public class ScanBarcodeControl : View
    {
        public static readonly BindableProperty BarcodeDetectedCommandProperty =
            BindableProperty.Create(
                nameof(BarcodeDetectedCommand),
                typeof(ICommand),
                typeof(ScanBarcodeControl),
                null);

        public ICommand BarcodeDetectedCommand
        {
            get => (ICommand)GetValue(BarcodeDetectedCommandProperty);
            set => SetValue(BarcodeDetectedCommandProperty, value);
        }

        //https:--//theconfuzedsourcecode.wordpress.com/2017/02/07/wait-how-could-i-handle-click-event-of-a-custom-renderer-button-in-xamarin-forms/

        public event EventHandler<ScanBarcodeControlBarcodeDetectedEventArgs> BarcodeDetected;

        public void SendBarcodeDetected(string barcode, bool serialized)
        {
            if(BarcodeDetected != null)
            {
                BarcodeDetected?.Invoke(this, new ScanBarcodeControlBarcodeDetectedEventArgs(barcode, serialized));
            }
        }
    }
}
