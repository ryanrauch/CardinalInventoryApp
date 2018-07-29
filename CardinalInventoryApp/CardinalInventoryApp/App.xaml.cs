using Autofac;
using CardinalInventoryApp.Contracts;
using CardinalInventoryApp.Views.ContentPages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace CardinalInventoryApp
{
    public partial class App : Application
    {
        public static IContainer Container { get; set; }
        public static ApplicationUserContract CurrentApplicationUserContract { get; set; }

        public App()
        {
            InitializeComponent();
            Container = AutoFacContainerBuilder.CreateContainer();
            //MainPage = new InitialView();
            //MainPage = new InventoryView();
            //MainPage = new ChartView();
            MainPage = new ScanBarcodeView();
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
