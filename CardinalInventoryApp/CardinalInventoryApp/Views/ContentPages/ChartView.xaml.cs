using CardinalInventoryApp.ViewModels;
using CardinalInventoryApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryApp.Views.ContentPages
{
    public class ChartViewBase : ViewPageBase<ChartViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChartView : ChartViewBase
	{
		public ChartView ()
		{
			InitializeComponent ();
		}
	}
}