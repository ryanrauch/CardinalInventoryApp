using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CardinalInventoryApp.Contracts;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Xamarin.Forms;

namespace CardinalInventoryApp.ViewModels
{
    public class SmartWatchViewModel : ViewModelBase
    {
        private const bool USESMARTWATCHSESSIONDATA = true;

        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IRequestService _requestService;
        private readonly IWatchSessionManager _watchSessionManager;
        private const int DISPLAYROWCOUNT = 12;
        private const double RADIANSTODEGREES = 180 / Math.PI;
        private const double MILLILITERTOOUNCE = 0.033814;

        private const double ROLLSTART = 80;
        private const double ROLLSTOP = 10;
        private const double PITCHSTART = 20;
        private const double PITCHSTOP = 0;

        private int _pourStartInterval { get; set; }
        private UInt64 _pourStartUnixTime { get; set; }
        private bool _isPouring { get; set; }

        private bool _debugMode { get; set; }

        public SmartWatchViewModel(
            IRequestService requestService,
            IWatchSessionManager watchSessionManager)
        {
            _debugMode = false;
            _initialAttitudeRoll = Double.MinValue;
            _updateIntervalSeconds = 0.0d;
            WristLocationString = string.Empty;
            _isPouring = false;
            _pourStartInterval = 0;
            _pourStartUnixTime = 0;
            PourSpouts = new ObservableCollection<PourSpout>();

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());

            ClearData();
            _requestService = requestService;
            _watchSessionManager = watchSessionManager;
            _watchSessionManager.DataReceived += _watchSessionManager_DataReceived;
        }

        public override void Initialize(object param)
        {
            base.Initialize(param);
            if(param is bool b)
            {
                _debugMode = b;
            }
        }

        public ICommand ClearDataCommand => new Command(ClearData);
        public ICommand SaveDataCommand => new Command(async () => await SaveDataAsync());

        public bool SaveDataEnabled => !IsBusy;

        private void ClearData()
        {
            if(USESMARTWATCHSESSIONDATA)
            {
                SessionDataList = new ObservableCollection<SmartWatchSessionData>();
            }
            else
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
                MeasuredPourLength = 0.0d;
                _pourStartUnixTime = 0;
            }
        }

        private async Task SaveDataAsync()
        {
            if(!_debugMode)
            {
                return;
            }
            IsBusy = true;
            var session = new SmartWatchSession()
            {
                SmartWatchSessionId = Guid.NewGuid(),
                Description = "SmartWatchViewModel2",
                IntervalDuration = Convert.ToDecimal(_updateIntervalSeconds),
                AttitudeRollOffset = _initialAttitudeRoll,
                Timestamp = DateTime.Now.ToUniversalTime(),
                IntervalStart = 0,
                IntervalStop = 0
            };
            if (SelectedPourSpout != null)
            {
                session.PourSpoutId = SelectedPourSpout.PourSpoutId;
            }
            if (!String.IsNullOrEmpty(WristLocationString)
               && String.Compare(WristLocationString, "Left", StringComparison.OrdinalIgnoreCase) == 0)
            {
                session.WristOrientation = SmartWatchWristOrientation.LeftHanded;
            }
            else
            {
                session.WristOrientation = SmartWatchWristOrientation.RightHanded;
            }
            await _requestService.PostAsync("SmartWatchSessions", session);

            if (USESMARTWATCHSESSIONDATA)
            {
                for (int i = 0; i < SessionDataList.Count; ++i)
                {
                    SessionDataList[i].SmartWatchSessionId = session.SmartWatchSessionId;
                    SessionDataList[i].Interval = i;
                    await _requestService.PostAsync("SmartWatchSessionData", SessionDataList[i]);
                }
            }
            else
            {
                for (int i = 0; i < _deviceMotionAttitudeListFull.Count; ++i)
                {
                    var data = new SmartWatchSessionData()
                    {
                        SmartWatchSessionId = session.SmartWatchSessionId,
                        Interval = i
                    };
                    // MotionAttitude
                    if (_deviceMotionAttitudeListFull[i].Contains(":"))
                    {
                        var splAttitude = _deviceMotionAttitudeListFull[i].Split(':');
                        if (splAttitude.Count() > 3)
                        {
                            data.AttitudePitch = Convert.ToDouble(splAttitude[0]);
                            data.AttitudeRoll = Convert.ToDouble(splAttitude[1]);
                            data.AttitudeYaw = Convert.ToDouble(splAttitude[2]);
                            data.TimestampUnixMs = Convert.ToUInt64(splAttitude[3]);
                        }
                    }
                    // RotationRate
                    if (_deviceMotionListFull.Count > i)
                    {
                        if (_deviceMotionListFull[i].Contains(":"))
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
                    // Accelerometer
                    if (_accelListFull.Count > i)
                    {
                        if (_accelListFull[i].Contains(":"))
                        {
                            var splRate = _accelListFull[i].Split(':');
                            if (splRate.Count() > 2)
                            {
                                data.AccelerometerX = Convert.ToDouble(splRate[0]);
                                data.AccelerometerY = Convert.ToDouble(splRate[1]);
                                data.AccelerometerZ = Convert.ToDouble(splRate[2]);
                            }
                        }
                    }
                    await _requestService.PostAsync("SmartWatchSessionData", data);
                }
            }
            ClearData();
            IsBusy = false;
        }

        private double _initialAttitudeRoll { get; set; } // Radians
        private double _updateIntervalSeconds { get; set; }
        private DateTime _sessionTimestamp { get; set; }

        private ObservableCollection<PourSpout> _pourSpouts { get; set; }
        public ObservableCollection<PourSpout> PourSpouts
        {
            get { return _pourSpouts; }
            set
            {
                _pourSpouts = value;
                RaisePropertyChanged(() => PourSpouts);
            }
        }

        public string SelectedPourSpoutDescription => SelectedPourSpout == null ? string.Empty : SelectedPourSpout.Description;
        private PourSpout _selectedPourSpout { get; set; }
        public PourSpout SelectedPourSpout
        {
            get { return _selectedPourSpout; }
            set
            {
                _selectedPourSpout = value;
                RaisePropertyChanged(() => SelectedPourSpout);
                RaisePropertyChanged(() => SelectedPourSpoutDescription);
            }
        }

        public string MeasuredPourVolumeOzString => MeasuredPourVolumeOz.ToString() + "oz";
        public double MeasuredPourVolumeOz
        {
            get { return MeasuredPourVolume * MILLILITERTOOUNCE; }
        }

        public string MeasuredPourVolumeString => MeasuredPourVolume.ToString() + "mL";
        public double MeasuredPourVolume
        {
            get
            {
                if(SelectedPourSpout == null)
                {
                    return 0.0d;
                }
                double mlPerSecond = 1000 / SelectedPourSpout.DurationForOneLiter;
                return MeasuredPourLength * mlPerSecond;
            }
        }

        public string MeasuredPourLengthString => MeasuredPourLength.ToString() + "sec";
        private double _measuredPourLength { get; set; }
        public double MeasuredPourLength
        {
            get { return _measuredPourLength; }
            set
            {
                _measuredPourLength = value;
                RaisePropertyChanged(() => MeasuredPourLength);
                RaisePropertyChanged(() => MeasuredPourLengthString);
                RaisePropertyChanged(() => MeasuredPourVolumeString);
                RaisePropertyChanged(() => MeasuredPourVolumeOzString);
            }
        }

        private ObservableCollection<SmartWatchSessionData> _sessionDataList { get; set; } = new ObservableCollection<SmartWatchSessionData>();
        public ObservableCollection<SmartWatchSessionData> SessionDataList
        {
            get { return _sessionDataList; }
            set
            {
                _sessionDataList = value;
                RaisePropertyChanged(() => SessionDataList);
            }
        }

        public string CurrentAttitudeRollString => CurrentAttitudeRoll.ToString();
        private double _currentAttitudeRoll { get; set; } // Degrees (0-360)
        public double CurrentAttitudeRoll 
        {
            get { return _currentAttitudeRoll; }
            set
            {
                _currentAttitudeRoll = value;
                RaisePropertyChanged(() => CurrentAttitudeRoll);
                RaisePropertyChanged(() => CurrentAttitudeRollString);
            }
        }

        public string CurrentAttitudePitchString => CurrentAttitudePitch.ToString();
        private double _currentAttitudePitch { get; set; } // Degrees (0-360)
        public double CurrentAttitudePitch
        {
            get { return _currentAttitudePitch; }
            set
            {
                _currentAttitudePitch = value;
                RaisePropertyChanged(() => CurrentAttitudePitch);
                RaisePropertyChanged(() => CurrentAttitudePitchString);
            }
        }

        public string CurrentAttitudeYawString => CurrentAttitudeYaw.ToString();
        private double _currentAttitudeYaw { get; set; } // Degrees (0-360)
        public double CurrentAttitudeYaw
        {
            get { return _currentAttitudeYaw; }
            set
            {
                _currentAttitudeYaw = value;
                RaisePropertyChanged(() => CurrentAttitudeYaw);
                RaisePropertyChanged(() => CurrentAttitudeYawString);
            }
        }

        private UInt64 _currentUnixTimestamp { get; set; }
        public UInt64 CurrentUnixTimestamp
        {
            get { return _currentUnixTimestamp; }
            set
            {
                _currentUnixTimestamp = value;
                RaisePropertyChanged(() => CurrentUnixTimestamp);
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
                        if(_debugMode)
                        {
                            GyroList.Add(e.Data);
                            _gyroListFull.Add(e.Data);
                            if (GyroList.Count > DISPLAYROWCOUNT)
                            {
                                GyroList.RemoveAt(0);
                            }
                        }
                        break;
                    case WatchDataType.AccelData:
                        if(_debugMode)
                        {
                            AccelList.Add(e.Data);
                            _accelListFull.Add(e.Data);
                            if (AccelList.Count > DISPLAYROWCOUNT)
                            {
                                AccelList.RemoveAt(0);
                            }
                        }
                        break;
                    case WatchDataType.DeviceMotionRotationRateData:
                        if(_debugMode)
                        {
                            DeviceMotionList.Add(e.Data);
                            _deviceMotionListFull.Add(e.Data);
                            if (DeviceMotionList.Count > DISPLAYROWCOUNT)
                            {
                                DeviceMotionList.RemoveAt(0);
                            }
                        }
                        break;
                    case WatchDataType.DeviceMotionAttitudeData:
                        SetCurrentAttitude(e.Data);
                        if(_debugMode)
                        {
                            DeviceMotionAttitudeList.Add(e.Data);
                            _deviceMotionAttitudeListFull.Add(e.Data);
                            if (DeviceMotionAttitudeList.Count > DISPLAYROWCOUNT)
                            {
                                DeviceMotionAttitudeList.RemoveAt(0);
                            }
                        }
                        break;
                    case WatchDataType.DeviceMotionAccelData:
                        if(_debugMode)
                        {
                            DeviceMotionAccelList.Add(e.Data);
                            _deviceMotionAccellListFull.Add(e.Data);
                            if (DeviceMotionAccelList.Count > DISPLAYROWCOUNT)
                            {
                                DeviceMotionAccelList.RemoveAt(0);
                            }
                        }
                        break;
                    case WatchDataType.InitializationData:
                        if (e.Data.Contains(":"))
                        {
                            var spl = e.Data.Split(':');
                            if (spl.Count() > 0)
                            {
                                _updateIntervalSeconds = Convert.ToDouble(spl[0]);
                                WristLocationString = spl[1];
                                _initialAttitudeRoll = Double.MinValue;
                            }
                        }
                        break;
                    case WatchDataType.SmartWatchSessionDataObj:
                        var swsd = JsonConvert.DeserializeObject<SmartWatchSessionData>(e.Data, _serializerSettings);
                        SetCurrentAttitude(swsd);
                        SessionDataList.Add(swsd);
                        break;
                    default:
                        break;
                }
            });
        }

        private void SetCurrentAttitude(SmartWatchSessionData data)
        {
            CurrentAttitudePitch = data.AttitudePitch * RADIANSTODEGREES;
            CurrentAttitudeRoll = data.AttitudeRoll * RADIANSTODEGREES;
            CurrentAttitudeYaw = data.AttitudeYaw * RADIANSTODEGREES;
            CurrentUnixTimestamp = data.TimestampUnixMs;
            CheckPouring();
        }

        private void SetCurrentAttitude(string data)
        {
            if(data.Contains(":"))
            {
                var spl = data.Split(':');
                if(spl.Count() > 3)
                {
                    var pitch = Convert.ToDouble(spl[0]);
                    CurrentAttitudePitch = pitch * RADIANSTODEGREES;
                    var roll = Convert.ToDouble(spl[1]);
                    CurrentAttitudeRoll = roll * RADIANSTODEGREES;
                    var yaw = Convert.ToDouble(spl[2]);
                    CurrentAttitudeYaw = yaw * RADIANSTODEGREES;
                    var uts = Convert.ToUInt64(spl[3]);
                    CurrentUnixTimestamp = uts;
                    CheckPouring();
                    if (_initialAttitudeRoll == Double.MinValue)
                    {
                        _initialAttitudeRoll = roll;
                    }
                }
            }
        }

        private void CheckPouring()
        {
            CheckPouringPitch();
            //CheckPouringRoll();
        }

        private void CheckPouringPitch()
        {
            if (!_isPouring)
            {
                if (CurrentAttitudePitch > PITCHSTART)
                {
                    _isPouring = true;
                    //_pourStartInterval = 0;
                    _pourStartUnixTime = CurrentUnixTimestamp;
                }
            }
            else
            {
                //_pourStartInterval++;
                //MeasuredPourLength = _pourStartInterval * _updateIntervalSeconds;
                MeasuredPourLength = (CurrentUnixTimestamp - _pourStartUnixTime) / 1000 * _updateIntervalSeconds;
                if (CurrentAttitudePitch < PITCHSTOP)
                {
                    _isPouring = false;
                    //_pourStartInterval = 0;
                    _pourStartUnixTime = 0;
                }
            }
        }

        private void CheckPouringRoll()
        {
            if(!_debugMode)
            {
                return; // must update counter interval function
            }
            if (!_isPouring)
            {
                if (CurrentAttitudeRoll > ROLLSTART)
                {
                    _isPouring = true;
                    _pourStartInterval = _deviceMotionAttitudeListFull.Count(); //called before record is added to collection
                }
            }
            else
            {
                MeasuredPourLength = (_deviceMotionAttitudeListFull.Count() - _pourStartInterval) * _updateIntervalSeconds;
                if (CurrentAttitudeRoll < ROLLSTOP)
                {
                    _isPouring = false;
                    _pourStartInterval = 0;
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
