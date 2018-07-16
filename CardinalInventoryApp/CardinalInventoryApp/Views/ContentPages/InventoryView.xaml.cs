using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class InventoryViewBase : ViewPageBase<InventoryViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InventoryView : InventoryViewBase
	{
		public InventoryView ()
		{
			InitializeComponent ();
		}
	}
}