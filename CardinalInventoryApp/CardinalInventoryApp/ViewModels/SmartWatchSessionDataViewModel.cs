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

        private const double RADIANSTODEGREES = 180 / Math.PI;
        private const double PITCHSTART = 20;
        private const double PITCHSTOP = 0;

        public SmartWatchSessionDataViewModel(
            IRequestService requestService,
            INavigationService navigationService)
        {
            _requestService = requestService;
            _navigationService = navigationService;
        }

        //public ICommand SmartWatchSessionsItemSelectedCommand => new Command<SmartWatchSession>(async (sws) => await SmartWatchSessionsItemSelectedAsync(sws));
        //public ICommand SmartWatchSessionsItemSelectedCommand => new Command<SmartWatchSession>(SmartWatchSessionsItemSelected);

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
                if(_selectedSmartWatchSession != value)
                {
                    _selectedSmartWatchSession = value;
                    if (_selectedSmartWatchSession != null)
                    {
                        Task.Run(async () => await SmartWatchSessionsItemSelectedAsync(_selectedSmartWatchSession));
                    }
                    RaisePropertyChanged(() => SelectedSmartWatchSession);
                }
            }
        }

        private async Task SmartWatchSessionsItemSelectedAsync(SmartWatchSession session)
        {
            SelectedSmartWatchSession = session;
            SmartWatchSessionData = await _requestService.GetAsync<List<SmartWatchSessionData>>("SmartWatchSessionData/" + SelectedSmartWatchSession.SmartWatchSessionId.ToString());
            var entriesPitch = new List<Microcharts.Entry>();
            var entriesRoll = new List<Microcharts.Entry>();
            var entriesYaw = new List<Microcharts.Entry>();
            var entriesUAX = new List<Microcharts.Entry>();
            var entriesUAY = new List<Microcharts.Entry>();
            var entriesUAZ = new List<Microcharts.Entry>();
            int total = SmartWatchSessionData.Count;
            int tenth = total / 10;
            ulong initialms = SmartWatchSessionData[0].TimestampUnixMs;
            bool pouring = false;
            List<int> keyIntervals = new List<int>();
            for (int i = 0; i < total; ++i)
            {
                var d = SmartWatchSessionData[i];
                if(d.AttitudePitch * RADIANSTODEGREES > PITCHSTART 
                   && !pouring)
                {
                    keyIntervals.Add(i);
                    pouring = true;
                }
                else if(d.AttitudePitch * RADIANSTODEGREES < PITCHSTOP
                       && pouring)
                {
                    keyIntervals.Add(i);
                    pouring = false;
                }
            }
            pouring = false;
            for (int i = 0; i < total; ++i)
            {
                var d = SmartWatchSessionData[i];
                //if(i % tenth == 0)
                if (keyIntervals.Contains(i))
                {
                    pouring = !pouring;
                    entriesPitch.Add(new Microcharts.Entry((float)d.AttitudePitch)
                    {
                        Label = ((d.TimestampUnixMs - initialms) / 1000d).ToString("0.##") + "s",
                        ValueLabel = (d.AttitudePitch * RADIANSTODEGREES).ToString("0.#") + "deg",
                        Color = pouring ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
                    });
                }
                else if (i == 0 || i == total - 1)
                {
                    entriesPitch.Add(new Microcharts.Entry((float)d.AttitudePitch)
                    {
                        Label = ((d.TimestampUnixMs - initialms) / 1000d).ToString("0.##") + "s",
                        ValueLabel = (d.AttitudePitch * RADIANSTODEGREES).ToString("0.#") + "deg",
                        Color = pouring ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
                    });
                }
                else
                {
                    entriesPitch.Add(new Microcharts.Entry((float)d.AttitudePitch)
                    {
                        Color = pouring ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
                    });
                }
                entriesRoll.Add(new Microcharts.Entry((float)d.AttitudeRoll)
                {
                    Color = pouring ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
                });
                entriesYaw.Add(new Microcharts.Entry((float)d.AttitudeYaw)
                {
                    Color = pouring ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
                });
                entriesUAX.Add(new Microcharts.Entry((float)d.UserAccelerationX)
                {
                    Color = pouring ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
                });
                entriesUAY.Add(new Microcharts.Entry((float)d.UserAccelerationY)
                {
                    Color = pouring ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
                });
                entriesUAZ.Add(new Microcharts.Entry((float)d.UserAccelerationZ)
                {
                    Color = pouring ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
                });
            }
            //foreach (var d in SmartWatchSessionData)
            //{
            //    entries.Add(new Microcharts.Entry((float)d.AttitudePitch)
            //    {
            //        Label = d.TimestampUnixMs.ToString(),
            //        ValueLabel = d.AttitudePitch.ToString(),
            //        Color = d.AttitudePitch > 0 ? SkiaSharp.SKColors.Red : SkiaSharp.SKColors.Black
            //    });
            //}
            Device.BeginInvokeOnMainThread(() => 
            {
                AttitudePitchChart = new LineChart()
                {
                    Entries = entriesPitch
                };
                AttitudeRollChart = new LineChart()
                {
                    Entries = entriesRoll
                };
                AttitudeYawChart = new LineChart()
                {
                    Entries = entriesYaw
                };

                UserAccelerationXChart = new LineChart()
                {
                    Entries = entriesUAX
                };
                UserAccelerationYChart = new LineChart()
                {
                    Entries = entriesUAY
                };
                UserAccelerationZChart = new LineChart()
                {
                    Entries = entriesUAZ
                };
            });
        }

        private Chart _attitudePitchChart { get; set; }
        public Chart AttitudePitchChart
        {
            get { return _attitudePitchChart; }
            set
            {
                _attitudePitchChart = value;
                RaisePropertyChanged(() => AttitudePitchChart);
            }
        }

        private Chart _attitudeRollChart { get; set; }
        public Chart AttitudeRollChart
        {
            get { return _attitudeRollChart; }
            set
            {
                _attitudeRollChart = value;
                RaisePropertyChanged(() => AttitudeRollChart);
            }
        }

        private Chart _attitudeYawChart { get; set; }
        public Chart AttitudeYawChart
        {
            get { return _attitudeYawChart; }
            set
            {
                _attitudeYawChart = value;
                RaisePropertyChanged(() => AttitudeYawChart);
            }
        }

        private Chart _userAccelerationXChart { get; set; }
        public Chart UserAccelerationXChart
        {
            get { return _userAccelerationXChart; }
            set
            {
                _userAccelerationXChart = value;
                RaisePropertyChanged(() => UserAccelerationXChart);
            }
        }

        private Chart _userAccelerationYChart { get; set; }
        public Chart UserAccelerationYChart
        {
            get { return _userAccelerationYChart; }
            set
            {
                _userAccelerationYChart = value;
                RaisePropertyChanged(() => UserAccelerationYChart);
            }
        }

        private Chart _userAccelerationZChart { get; set; }
        public Chart UserAccelerationZChart
        {
            get { return _userAccelerationZChart; }
            set
            {
                _userAccelerationZChart = value;
                RaisePropertyChanged(() => UserAccelerationZChart);
            }
        }

        public override async Task OnAppearingAsync()
        {
            SmartWatchSessions = await _requestService.GetAsync<ObservableCollection<SmartWatchSession>>("SmartWatchSessions");
            //SmartWatchSessionData = await _requestService.GetAsync<List<SmartWatchSessionData>>("SmartWatchSessionData");
            if(SmartWatchSessions.Count > 0)
            {
                SelectedSmartWatchSession = SmartWatchSessions[0];
            }
        }
    }
}
