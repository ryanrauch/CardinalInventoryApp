using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class SmartWatchSessionDataViewBase : ViewPageBase<SmartWatchSessionDataViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SmartWatchSessionDataView : SmartWatchSessionDataViewBase
	{
		public SmartWatchSessionDataView ()
		{
			InitializeComponent ();
		}
	}
}