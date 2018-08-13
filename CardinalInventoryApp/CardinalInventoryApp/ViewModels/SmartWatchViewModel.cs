using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CardinalInventoryApp.Contracts;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using Xamarin.Forms;

namespace CardinalInventoryApp.ViewModels
{
    public class SmartWatchViewModel : ViewModelBase
    {
        private readonly IRequestService _requestService;
        private readonly IWatchSessionManager _watchSessionManager;
        private const int DISPLAYROWCOUNT = 14;
        private const double RADIANSTODEGREES = 180 / Math.PI;

        public SmartWatchViewModel(
            IRequestService requestService,
            IWatchSessionManager watchSessionManager)
        {
            _initialAttitudeRoll = Double.MinValue;
            _updateIntervalSeconds = 0.0d;
            WristLocationString = string.Empty;

            ClearData();
            _requestService = requestService;
            _watchSessionManager = watchSessionManager;
            _watchSessionManager.DataReceived += _watchSessionManager_DataReceived;
        }

        public ICommand ClearDataCommand => new Command(ClearData);
        public ICommand SaveDataCommand => new Command(async () => await SaveDataAsync());

        public bool SaveDataEnabled => !IsBusy;

        private void ClearData()
        {
            _sessionTimestamp = DateTime.Now;
            _gyroListFull = new List<string>();
            _accelListFull = new List<string>();
            _deviceMotionListFull = new List<string>();
            _deviceMotionAttitudeListFull = new List<string>();
            _deviceMotionAccellListFull = new List<string>();

            GyroList = new ObservableCollection<string>();
            AccelList = new ObservableCollection<string>();
            DeviceMotionList = new ObservableCollection<string>();
            DeviceMotionAttitudeList = new ObservableCollection<string>();
            DeviceMotionAccelList = new ObservableCollection<string>();
            CurrentAttitudeRoll = 0.0d;
        }

        private async Task SaveDataAsync()
        {
            IsBusy = true;
            var session = new SmartWatchSession()
            {
                SmartWatchSessionId = Guid.NewGuid(),
                Description = "SmartWatchViewModel",
                IntervalDuration = Convert.ToDecimal(_updateIntervalSeconds),
                AttitudeRollOffset = _initialAttitudeRoll,
                Timestamp = DateTime.Now,
                IntervalStart = 0,
                IntervalStop = 0
            };
            if(SelectedPourSpout != null)
            {
                session.PourSpoutId = SelectedPourSpout.PourSpoutId;
            }
            await _requestService.PostAsync("SmartWatchSessions", session);

            for (int i = 0; i < _deviceMotionAttitudeListFull.Count; ++i)
            {
                var data = new SmartWatchSessionData()
                {
                    SmartWatchSessionId = session.SmartWatchSessionId,
                    Interval = i
                };
                // MotionAttitude
                if(_deviceMotionAttitudeListFull[i].Contains(":"))
                {
                    var splAttitude = _deviceMotionAttitudeListFull[i].Split(':');
                    if (splAttitude.Count() > 2)
                    {
                        data.AttitudePitch = Convert.ToDouble(splAttitude[0]);
                        data.AttitudeRoll = Convert.ToDouble(splAttitude[1]);
                        data.AttitudeYaw = Convert.ToDouble(splAttitude[2]);
                    }
                }
                // RotationRate
                if(_deviceMotionListFull.Count > i)
                {
                    if(_deviceMotionListFull[i].Contains(":"))
                    {
                        var splRate = _deviceMotionListFull[i].Split(':');
                        if (splRate.Count() > 2)
                        {
                            data.RotationRateX = Convert.ToDouble(splRate[0]);
                            data.RotationRateY = Convert.ToDouble(splRate[1]);
                            data.RotationRateZ = Convert.ToDouble(splRate[2]);
                        }
                    }
                }
                // UserAcceleration
                if (_deviceMotionAccellListFull.Count > i)
                {
                    if (_deviceMotionAccellListFull[i].Contains(":"))
                    {
                        var splRate = _deviceMotionAccellListFull[i].Split(':');
                        if (splRate.Count() > 2)
                        {
                            data.UserAccelerationX = Convert.ToDouble(splRate[0]);
                            data.UserAccelerationY = Convert.ToDouble(splRate[1]);
                            data.UserAccelerationZ = Convert.ToDouble(splRate[2]);
                        }
                    }
                }
                await _requestService.PostAsync("SmartWatchSessionData", data);
            }
            ClearData();
            IsBusy = false;
        }

        private double _initialAttitudeRoll { get; set; } // Radians
        private double _updateIntervalSeconds { get; set; }
        private DateTime _sessionTimestamp { get; set; }

        private ObservableCollection<PourSpout> _pourSpouts { get; set; } = new ObservableCollection<PourSpout>();
        public ObservableCollection<PourSpout> PourSpouts
        {
            get { return _pourSpouts; }
            set
            {
                _pourSpouts = value;
                RaisePropertyChanged(() => PourSpouts);
            }
        }

        public string SelectedPourSpoutDescription => SelectedPourSpout?.Description;
        private PourSpout _selectedPourSpout { get; set; }
        public PourSpout SelectedPourSpout
        {
            get { return _selectedPourSpout; }
            set
            {
                _selectedPourSpout = value;
                RaisePropertyChanged(() => SelectedPourSpout);
            }
        }

        private double _currentAttitudeRoll { get; set; } // Degrees (0-360)
        public double CurrentAttitudeRoll 
        {
            get { return _currentAttitudeRoll; }
            set
            {
                _currentAttitudeRoll = value;
                RaisePropertyChanged(() => CurrentAttitudeRoll);
            }
        }

        private string _wristLocationString { get; set; }
        public string WristLocationString
        {
            get { return _wristLocationString; }
            set
            {
                _wristLocationString = value;
                RaisePropertyChanged(() => WristLocationString);
            }
        }

        private List<string> _gyroListFull { get; set; }
        private ObservableCollection<string> _gyroList { get; set; }
        public ObservableCollection<string> GyroList
        {
            get { return _gyroList; }
            set
            {
                _gyroList = value;
                RaisePropertyChanged(() => GyroList);
            }
        }

        private List<string> _accelListFull { get; set; }
        private ObservableCollection<string> _accelList { get; set; }
        public ObservableCollection<string> AccelList
        {
            get { return _accelList; }
            set
            {
                _accelList = value;
                RaisePropertyChanged(() => AccelList);
            }
        }

        private List<string> _deviceMotionListFull { get; set; }
        private ObservableCollection<string> _deviceMotionList { get; set; }
        public ObservableCollection<string> DeviceMotionList
        {
            get { return _deviceMotionList; }
            set
            {
                _deviceMotionList = value;
                RaisePropertyChanged(() => DeviceMotionList);
            }
        }

        private List<string> _deviceMotionAttitudeListFull { get; set; }
        private ObservableCollection<string> _deviceMotionAttitudeList { get; set; }
        public ObservableCollection<string> DeviceMotionAttitudeList
        {
            get { return _deviceMotionAttitudeList; }
            set
            {
                _deviceMotionAttitudeList = value;
                RaisePropertyChanged(() => DeviceMotionAttitudeList);
            }
        }

        private List<string> _deviceMotionAccellListFull { get; set; }
        private ObservableCollection<string> _deviceMotionAccellList { get; set; }
        public ObservableCollection<string> DeviceMotionAccelList
        {
            get { return _deviceMotionAccellList; }
            set
            {
                _deviceMotionAccellList = value;
                RaisePropertyChanged(() => DeviceMotionAccelList);
            }
        }
        private void _watchSessionManager_DataReceived(object sender, WatchDataEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                switch (e.WatchDataType)
                {
                    case WatchDataType.GyroData:
                        GyroList.Add(e.Data);
                        _gyroListFull.Add(e.Data);
                        if(GyroList.Count > DISPLAYROWCOUNT)
                        {
                            GyroList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.AccelData:
                        AccelList.Add(e.Data);
                        _accelListFull.Add(e.Data);
                        if(AccelList.Count > DISPLAYROWCOUNT)
                        {
                            AccelList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviceMotionRotationRateData:
                        DeviceMotionList.Add(e.Data);
                        _deviceMotionListFull.Add(e.Data);
                        if(DeviceMotionList.Count > DISPLAYROWCOUNT)
                        {
                            DeviceMotionList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviceMotionAttitudeData:
                        SetCurrentAttitudeRoll(e.Data);
                        DeviceMotionAttitudeList.Add(e.Data);
                        _deviceMotionAttitudeListFull.Add(e.Data);
                        if (DeviceMotionAttitudeList.Count > DISPLAYROWCOUNT)
                        {
                            DeviceMotionAttitudeList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviceMotionAccelData:
                        DeviceMotionAccelList.Add(e.Data);
                        _deviceMotionAccellListFull.Add(e.Data);
                        if (DeviceMotionAccelList.Count > DISPLAYROWCOUNT)
                        {
                            DeviceMotionAccelList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.InitializationData:
                        if(e.Data.Contains(":"))
                        {
                            var spl = e.Data.Split(':');
                            if(spl.Count() > 0)
                            {
                                _updateIntervalSeconds = Convert.ToDouble(spl[0]);
                                WristLocationString = spl[1];
                                _initialAttitudeRoll = Double.MinValue;
                            }
                        }
                        break;
                    default:
                        break;
                }
            });
        }

        private void SetCurrentAttitudeRoll(string data)
        {
            if(data.Contains(":"))
            {
                var spl = data.Split(':');
                if(spl.Count() > 1)
                {
                    var roll = Convert.ToDouble(spl[1]);
                    CurrentAttitudeRoll = roll * RADIANSTODEGREES;
                    if(_initialAttitudeRoll == Double.MinValue)
                    {
                        _initialAttitudeRoll = roll;
                    }
                }
            }
        }

        public override async Task OnAppearingAsync()
        {
            _watchSessionManager.StartSession();
            if(PourSpouts.Count == 0)
            {
                var spouts = await _requestService.GetAsync<List<PourSpout>>("PourSpouts");
                foreach (var s in spouts)
                {
                    if (SelectedPourSpout == null)
                    {
                        SelectedPourSpout = s;
                    }
                    PourSpouts.Add(s);
                }
            }
        }
    }
}
