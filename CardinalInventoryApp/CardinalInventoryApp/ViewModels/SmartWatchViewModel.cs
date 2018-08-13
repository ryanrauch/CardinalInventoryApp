using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
            ClearData();
            _requestService = requestService;
            _watchSessionManager = watchSessionManager;
            _watchSessionManager.DataReceived += _watchSessionManager_DataReceived;
        }

        public ICommand ClearDataCommand => new Command(ClearData);
        public ICommand SaveDataCommand => new Command(async () => await SaveDataAsync());

        private void ClearData()
        {
            GyroList = new ObservableCollection<string>();
            AccelList = new ObservableCollection<string>();
            DeviceMotionList = new ObservableCollection<string>();
            DeviceMotionAttitudeList = new ObservableCollection<string>();
            DeviceMotionAccelList = new ObservableCollection<string>();
            WristLocationString = string.Empty;
            CurrentAttitudeRoll = 0.0d;
        }

        private async Task SaveDataAsync()
        {
            await Task.Delay(10);
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
                        if(GyroList.Count > DISPLAYROWCOUNT)
                        {
                            GyroList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.AccelData:
                        AccelList.Add(e.Data);
                        if(AccelList.Count > DISPLAYROWCOUNT)
                        {
                            AccelList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviceMotionRotationRateData:
                        DeviceMotionList.Add(e.Data);
                        if(DeviceMotionList.Count > DISPLAYROWCOUNT)
                        {
                            DeviceMotionList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviceMotionAttitudeData:
                        SetCurrentAttitudeRoll(e.Data);
                        DeviceMotionAttitudeList.Add(e.Data);
                        if (DeviceMotionAttitudeList.Count > DISPLAYROWCOUNT)
                        {
                            DeviceMotionAttitudeList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviveMotionAccelData:
                        DeviceMotionAccelList.Add(e.Data);
                        if (DeviceMotionAccelList.Count > DISPLAYROWCOUNT)
                        {
                            DeviceMotionAccelList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.WristLocationData:
                        WristLocationString = e.Data;
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
                }
            }
        }

        public override Task OnAppearingAsync()
        {
            _watchSessionManager.StartSession();
            return Task.CompletedTask;
        }
    }
}
