using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class SmartWatchViewBase : ViewPageBase<SmartWatchViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SmartWatchView : SmartWatchViewBase
	{
		public SmartWatchView ()
		{
			InitializeComponent ();
		}
	}
}