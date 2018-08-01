using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class ReceiveItemViewBase : ViewPageBase<ReceiveItemViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ReceiveItemView : ReceiveItemViewBase
	{
		public ReceiveItemView ()
		{
			InitializeComponent ();
		}
	}
}