﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;

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
            _watchSessionManager.DataReceived += _watchSessionManager_DataReceived;
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
            int rowCount = 14;
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                switch (e.WatchDataType)
                {
                    case WatchDataType.GyroData:
                        GyroList.Add(e.Data);
                        if(GyroList.Count > rowCount)
                        {
                            GyroList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.AccelData:
                        AccelList.Add(e.Data);
                        if(AccelList.Count > rowCount)
                        {
                            AccelList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviceMotionRotationRateData:
                        DeviceMotionList.Add(e.Data);
                        if(DeviceMotionList.Count > rowCount)
                        {
                            DeviceMotionList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviceMotionAttitudeData:
                        DeviceMotionAttitudeList.Add(e.Data);
                        if (DeviceMotionAttitudeList.Count > rowCount)
                        {
                            DeviceMotionAttitudeList.RemoveAt(0);
                        }
                        break;
                    case WatchDataType.DeviveMotionAccelData:
                        DeviceMotionAccelList.Add(e.Data);
                        if (DeviceMotionAccelList.Count > rowCount)
                        {
                            DeviceMotionAccelList.RemoveAt(0);
                        }
                        break;
                    default:
                        break;
                }
            });
        }

        public override Task OnAppearingAsync()
        {
            GyroList = new ObservableCollection<string>();
            AccelList = new ObservableCollection<string>();
            DeviceMotionList = new ObservableCollection<string>();
            DeviceMotionAttitudeList = new ObservableCollection<string>();
            DeviceMotionAccelList = new ObservableCollection<string>();
            _watchSessionManager.StartSession();
            return Task.CompletedTask;
        }
    }
}
