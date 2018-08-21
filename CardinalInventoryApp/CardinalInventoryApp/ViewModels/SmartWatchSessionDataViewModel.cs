using CardinalInventoryApp.Contracts;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using Microcharts;
using Microcharts.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

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

        public ICommand SmartWatchSessionsItemSelectedCommand => new Command<SmartWatchSession>(SmartWatchSessionsItemSelected);

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

        private List<SmartWatchSessionData> _smartWatchSessionData { get; set; }
        public List<SmartWatchSessionData> SmartWatchSessionData
        {
            get { return _smartWatchSessionData; }
            set
            {
                _smartWatchSessionData = value;
                RaisePropertyChanged(() => SmartWatchSessionData);
            }
        }

        private SmartWatchSession _selectedSmartWatchSession { get; set; }
        public SmartWatchSession SelectedSmartWatchSession
        {
            get { return _selectedSmartWatchSession; }
            set
            {
                _selectedSmartWatchSession = value;
                RaisePropertyChanged(() => SelectedSmartWatchSession);
            }
        }

        private void SmartWatchSessionsItemSelected(SmartWatchSession session)
        {
            SelectedSmartWatchSession = session;
            var data = SmartWatchSessionData.Where(s => s.SmartWatchSessionId.Equals(session.SmartWatchSessionId)).ToList();
            var entries = new List<Microcharts.Entry>();
            foreach (var d in data)
            {
                entries.Add(new Microcharts.Entry((float)d.AttitudePitch));
            }
            AttitudePitchChart = new ChartView()
            {
                Chart = new LineChart() { Entries = entries }
            };
        }

        private ChartView _attitudePitchChart { get; set; }
        public ChartView AttitudePitchChart
        {
            get { return _attitudePitchChart; }
            set
            {
                _attitudePitchChart = value;
                RaisePropertyChanged(() => AttitudePitchChart);
            }
        }

        public override async Task OnAppearingAsync()
        {
            SmartWatchSessions = await _requestService.GetAsync<ObservableCollection<SmartWatchSession>>("SmartWatchSessions");
            SmartWatchSessionData = await _requestService.GetAsync<List<SmartWatchSessionData>>("SmartWatchSessionData");
        }
    }
}
