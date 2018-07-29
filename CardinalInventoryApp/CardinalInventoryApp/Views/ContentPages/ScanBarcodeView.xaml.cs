using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class ScanBarcodeViewBase : ViewPageBase<ScanBarcodeViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScanBarcodeView : ScanBarcodeViewBase
	{
		public ScanBarcodeView ()
		{
			InitializeComponent ();
		}
	}
}