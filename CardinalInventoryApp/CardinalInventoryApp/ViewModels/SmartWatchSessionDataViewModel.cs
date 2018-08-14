using CardinalInventoryApp.Contracts;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CardinalInventoryApp.ViewModels
{
    public class SmartWatchSessionDataViewModel : ViewModelBase
    {
        private readonly IRequestService _requestService;
        private readonly INavigationService _navigationService;

        public SmartWatchSessionDataViewModel(
            IRequestService requestService,
            INavigationService navigationService)
        {
            _requestService = requestService;
            _navigationService = navigationService;
        }

        private ObservableCollection<SmartWatchSession> _smartWatchSessions { get; set; } = new ObservableCollection<SmartWatchSession>();
        public ObservableCollection<SmartWatchSession> SmartWatchSessions
        {
            get { return _smartWatchSessions; }
            set
            {
                _smartWatchSessions = value;
                RaisePropertyChanged(() => SmartWatchSessions);
            }
        }

        public override async Task OnAppearingAsync()
        {
            SmartWatchSessions = await _requestService.GetAsync<ObservableCollection<SmartWatchSession>>("SmartWatchSessions");
            
        }
    }
}
