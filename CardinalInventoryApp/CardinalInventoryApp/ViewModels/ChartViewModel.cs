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

        public ChartViewModel(IRequestService requestService)
        {
            _requestService = requestService;
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

        public Chart InventoryActionChart => new LineChart()
        {
            Entries= new[]
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
                 }
            },
            LineMode = LineMode.Straight,
            PointMode = PointMode.None,
            LineSize = 8,
            BackgroundColor = SKColors.Transparent,
        };

        public override Task OnAppearingAsync()
        {
            var entries = new[]
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
            var mc = new LineChart
            {
                Entries = entries
            };

            MotionChart = mc;
            return Task.CompletedTask;
        }
    }
}
