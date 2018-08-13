using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class DirectionalViewBase : ViewPageBase<SmartWatchViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DirectionalView : DirectionalViewBase
	{
		public DirectionalView ()
		{
			InitializeComponent ();
		}
	}
}