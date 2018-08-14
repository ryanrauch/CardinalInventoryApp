using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using CardinalInventoryApp.Views.ContentPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CardinalInventoryApp.ViewModels
{
    public class InitialViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public InitialViewModel(
            INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public ICommand SmartWatchViewCommand => new Command(() => _navigationService.NavigatePushAsync(new SmartWatchView()));
        public ICommand SmartWatchSessionDataViewCommand => new Command(() => _navigationService.NavigatePushAsync(new SmartWatchSessionDataView()));
        public ICommand DirectionalViewCommand => new Command(() => _navigationService.NavigatePushAsync(new DirectionalView()));
        public ICommand ReceiveItemViewCommand => new Command(() => _navigationService.NavigatePushAsync(new ReceiveItemView()));
        public ICommand ScanBarcodeViewCommand => new Command(() => _navigationService.NavigatePushAsync(new ScanBarcodeView()));
        public ICommand InventoryViewCommand => new Command(() => _navigationService.NavigatePushAsync(new InventoryView()));

        public override Task OnAppearingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
