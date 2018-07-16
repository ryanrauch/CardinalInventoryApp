using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class InventoryCompletedViewBase : ViewPageBase<InventoryCompletedViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InventoryCompletedView : InventoryCompletedViewBase
	{
		public InventoryCompletedView ()
		{
			InitializeComponent ();
		}
	}
}