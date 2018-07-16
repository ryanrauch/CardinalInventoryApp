using CardinalInventoryApp.Contracts;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CardinalInventoryApp.ViewModels
{
    public class InventoryCompletedViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public InventoryCompletedViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        private Area _currentArea { get; set; }

        public ICommand GotoMainCommand => new Command(GotoMainTask);

        private void GotoMainTask()
        {
            _navigationService.NavigateToMain();
        }

        public override void Initialize(object param)
        {
            base.Initialize(param);
            if(param is Area a)
            {
                _currentArea = a;
            }
        }

        public override Task OnAppearingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
