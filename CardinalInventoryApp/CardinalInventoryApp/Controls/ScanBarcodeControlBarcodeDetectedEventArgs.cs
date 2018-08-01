using System;
using System.Collections.Generic;
using System.Text;

namespace CardinalInventoryApp.Controls
{
    public class ScanBarcodeControlBarcodeDetectedEventArgs : EventArgs
    {
        public string Barcode { get; set; }
        public bool SerializedStockItem { get; set; }
        public ScanBarcodeControlBarcodeDetectedEventArgs(string barcode, bool serializedStockItem)
        {
            Barcode = barcode;
            SerializedStockItem = serializedStockItem;
        }
    }
}
