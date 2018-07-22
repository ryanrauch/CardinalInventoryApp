using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using Microcharts;
using SkiaSharp;

namespace CardinalInventoryApp.ViewModels
{
    public class ChartViewModel : ViewModelBase
    {
        private readonly IRequestService _requestService;
        private readonly IWatchSessionManager _watchSessionManager;

        public ChartViewModel(
            IRequestService requestService,
            IWatchSessionManager watchSessionManager)
        {
            _requestService = requestService;
            _watchSessionManager = watchSessionManager;
            //_watchSessionManager = Xamarin.Forms.DependencyService.Get<IWatchSessionManager>();
            _watchSessionManager.DataReceived += _watchSessionManager_DataReceived;
        }

        private void _watchSessionManager_DataReceived(object sender, WatchDataEventArgs e)
        {
            switch(e.WatchDataType)
            {
                case WatchDataType.GyroDataX:
                    _entries.Add(new Entry((float)Convert.ToDouble(e.Data)));
                    break;
                default:
                    _entries.Add(new Entry(-33.0f));
                    break;
            }
            UpdateMotionChart();
        }

        private List<Entry> _entries { get; set; } = new List<Entry>();

        private Chart _motionChart { get; set; } = new LineChart();
        public Chart MotionChart
        {
            get { return _motionChart; }
            set
            {
                _motionChart = value;
                RaisePropertyChanged(() => MotionChart);
            }
        }

        private void UpdateMotionChart()
        {
            var mc = new LineChart
            {
                Entries = _entries
            };
            MotionChart = mc;
        }

        public override Task OnAppearingAsync()
        {
             _entries = new List<Entry>
             {
                 new Entry(212)
                 {
                     Label = "UWP",
                     ValueLabel = "212",
                     Color = SKColor.Parse("#2c3e50")
                 },
                 new Entry(248)
                 {
                     Label = "Android",
                     ValueLabel = "248",
                     Color = SKColor.Parse("#77d065")
                 },
                 new Entry(128)
                 {
                     Label = "iOS",
                     ValueLabel = "128",
                     Color = SKColor.Parse("#b455b6")
                 },
                 new Entry(514)
                 {
                     Label = "Shared",
                     ValueLabel = "514",
                     Color = SKColor.Parse("#3498db")
                 }
            };
            UpdateMotionChart();
            return Task.CompletedTask;
        }
    }
}
