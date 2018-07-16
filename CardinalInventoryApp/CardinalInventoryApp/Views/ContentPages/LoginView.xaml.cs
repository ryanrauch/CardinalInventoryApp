using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class LoginViewBase : ViewPageBase<LoginViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginView : LoginViewBase
	{
		public LoginView ()
		{
			InitializeComponent ();
		}
	}
}