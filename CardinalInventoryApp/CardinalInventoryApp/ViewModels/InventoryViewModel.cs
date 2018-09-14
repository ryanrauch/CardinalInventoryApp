using CardinalInventoryApp.Contracts;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CardinalInventoryApp.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IRequestService _requestService;
        private readonly INavigationService _navigationService;

        public InventoryViewModel(
            IBlobStorageService blobStorageService,
            IRequestService requestService,
            INavigationService navigationService)
        {
            _blobStorageService = blobStorageService;
            _requestService = requestService;
            _navigationService = navigationService;

            for (int i = 5; i > 0; --i)
            {
                StockItemLevels.Add(new StockItemLevelViewModel(0.2M * i));
            }
        }

        private Area _currentArea { get; set; }

        private int _stockItemIndex { get; set; } = 0;

        //public ICommand SkipStockItemCommand => new Command(async () => await SkipStockItemTask());
        public ICommand NextStockItemCommand => new Command(async () => await NextStockItemTask());
        public ICommand QuantityIncreaseCommand => new Command(async () => await IncreaseStockItemTask());
        public ICommand QuantityDecreaseCommand => new Command(async () => await DecreaseStockItemTask());
        public ICommand SelectItemLevelCommand => new Command<StockItemLevelViewModel>(async (svm) => await IncreaseStockItemTask(svm));

        private ObservableCollection<StockItemLevelViewModel> _stockItemLevels { get; set; } = new ObservableCollection<StockItemLevelViewModel>();
        public ObservableCollection<StockItemLevelViewModel> StockItemLevels
        {
            get { return _stockItemLevels; }
            set
            {
                _stockItemLevels = value;
                RaisePropertyChanged(() => StockItemLevels);
            }
        }

        private DateTime _startedDateTime { get; set; } = DateTime.MinValue;
        public DateTime StartedDateTime
        {
            get { return _startedDateTime; }
            set
            {
                _startedDateTime = value;
                RaisePropertyChanged(() => StartedDateTime);
            }
        }

        private int _totalItemsCounted { get; set; } = 0;
        public int TotalItemsCounted
        {
            get { return _totalItemsCounted; }
            set
            {
                _totalItemsCounted = value;
                RaisePropertyChanged(() => TotalItemsCounted);
            }
        }

        public String TotalItemsCountedMessage
        {
            get
            {
                if (StartedDateTime > DateTime.Now.AddMinutes(-1))
                {
                    return String.Format("{0} Total Bottles Inventoried in {1}mins",
                                            TotalItemsCounted,
                                            (int)(DateTime.Now.Subtract(StartedDateTime).TotalMinutes));
                }
                return String.Empty;
            }
        }

        private DateTime _statusMessageSet { get; set; } = DateTime.MinValue;

        private String _statusMessage { get; set; } = String.Empty;
        public String StatusMessage
        {
            get
            {
                String smt = String.Empty;
                if(_statusMessageSet.Equals(DateTime.MinValue))
                {
                    return smt;
                }
                else if(_statusMessageSet > DateTime.Now.AddSeconds(-15))
                {
                    smt = "Just Now";
                }
                else if(_statusMessageSet > DateTime.Now.AddMinutes(-2))
                {
                    smt = "1min ago";
                }
                else if (_statusMessageSet > DateTime.Now.AddHours(-1))
                {
                    smt = String.Format("{0}mins ago", (int)(DateTime.Now.Subtract(_statusMessageSet).TotalMinutes));
                }
                else
                {
                    smt = String.Format("{0}hour(s) ago", (int)(DateTime.Now.Subtract(_statusMessageSet).TotalHours));
                }
                return String.Format("{0} [{1}]", _statusMessage,
                                                  smt);
            }
            set
            {
                _statusMessage = value;
                _statusMessageSet = DateTime.Now;
                RaisePropertyChanged(() => StatusMessage);
            }
        }

        public String SelectedItemCountMessage => String.Format("QTY: {0}", SelectedItemCount);

        private int _selectedItemCount { get; set; } = 0;
        public int SelectedItemCount
        {
            get { return _selectedItemCount; }
            set
            {
                _selectedItemCount = value;
                RaisePropertyChanged(() => SelectedItemCount);
                RaisePropertyChanged(() => SelectedItemCountMessage);
            }
        }

        private ImageSource _selectedImageSource { get; set; } = null;
        public ImageSource SelectedImageSource
        {
            get { return _selectedImageSource; }
            set
            {
                _selectedImageSource = value;
                RaisePropertyChanged(() => SelectedImageSource);
            }
        }

        private StockItem _selectedStockItem { get; set; } = null;
        public StockItem SelectedStockItem
        {
            get { return _selectedStockItem; }
            set
            {
                _selectedStockItem = value;
                SelectedItemCount = 0;
                RaisePropertyChanged(() => SelectedStockItem);
            }
        }

        private Decimal _selectedItemLevel { get; set; } = 1.0M;
        public Decimal SelectedItemLevel
        {
            get { return _selectedItemLevel; }
            set
            {
                _selectedItemLevel = value;
                RaisePropertyChanged(() => SelectedItemLevel);
            }
        }

        private async Task<bool> CreateInventoryActionHistory(Decimal level, InventoryAction action)
        {
            Debug.WriteLine("InventoryViewModel::CreateInventoryActionHistory() - Not finished");
            return true;
            var ah = new InventoryActionHistory()
            {
                InventoryActionHistoryId = Guid.NewGuid(),
                Action = action,
                //SerializedStockItemId = //SelectedStockItem.StockItemId,
                Timestamp = DateTime.Now,
                ItemLevel = level,
                ApplicationUserId = App.CurrentApplicationUserContract.Id,
                AreaId = _currentArea.AreaId
            };
            SelectedItemLevel = level;
            var res = await _requestService.PostAsync("InventoryActionHistories", ah);
            return res != null;
        }

        private async Task NextStockItemTask()
        {
            StatusMessage = "Next Item Loaded";
            if (++_stockItemIndex < _stockItems.Count)
            {
                SelectedStockItem = _stockItems[_stockItemIndex];
                SelectedImageSource = _stockItemImages[_stockItemIndex];
                if (_stockItemIndex > 0)
                {
                    //clear out previous images
                    _stockItemImages[_stockItemIndex - 1] = null;
                }
            }
            else
            {
                await _navigationService.NavigatePushAsync(new Views.ContentPages.InventoryCompletedView(), _currentArea);
            }
        }

        private bool OnTimer()
        {
            RaisePropertyChanged(() => TotalItemsCountedMessage);
            RaisePropertyChanged(() => StatusMessage);
            return true;
        }

        private async Task IncreaseStockItemTask(StockItemLevelViewModel svm)
        {
            if (!await CreateInventoryActionHistory(svm.ItemLevel, InventoryAction.UserViewedAuto))
            {
                StatusMessage = "CreateInventoryActionHistory::Failed";
            }
            else
            {
                StatusMessage = String.Format("{0} Inventory Saved", svm.LevelText);
            }
            SelectedItemCount++;
            TotalItemsCounted++;
            SelectedImageSource = _stockItemImages[_stockItemIndex];
        }

        private async Task IncreaseStockItemTask()
        {
            if(!await CreateInventoryActionHistory(1.0M, InventoryAction.UserViewedAuto))
            {
                StatusMessage = "CreateInventoryActionHistory::Failed";
            }
            else
            {
                StatusMessage = "Inventory Saved";
            }
            SelectedItemCount++;
            TotalItemsCounted++;
            SelectedImageSource = _stockItemImages[_stockItemIndex];
        }

        private async Task DecreaseStockItemTask()
        {
            if (!await CreateInventoryActionHistory(SelectedItemLevel, InventoryAction.RemovedDuringInventory))
            {
                StatusMessage = "CreateInventoryActionHistory::Remove::Failed";
            }
            else
            {
                StatusMessage = "Inventory Removed";
            }
            SelectedItemCount--;
            TotalItemsCounted--;
            SelectedImageSource = _stockItemImages[_stockItemIndex];
        }

        /************/
        //Data-Repository
        private List<ApplicationUserContract> _applicationUserContracts { get; set; }
        private List<Bar> _bars { get; set; }
        private List<Building> _buildings { get; set; }
        private List<Area> _areas { get; set; }
        //private List<StockItemCategory> _stockItemCategories { get; set; }
        private List<SerializedStockItem> _serializedStockItems { get; set; }
        private List<StockItemTag> _stockItemTags { get; set; }
        private List<StockItem> _stockItems { get; set; }
        private ImageSource[] _stockItemImages { get; set; }
        //inventory & actions?
        private List<CloudBlockBlob> _blobs { get; set; }
        /************/

        public override void Initialize(object param)
        {
            base.Initialize(param);
            if(param is Area a)
            {
                _currentArea = a;
            }
        }

        public override async Task OnAppearingAsync()
        {
            var reqApplicationUsers = _requestService.GetAsync<List<ApplicationUserContract>>("ApplicationUsers");
            var reqBars = _requestService.GetAsync<List<Bar>>("Bars");
            var reqBuildings = _requestService.GetAsync<List<Building>>("Buildings");
            var reqAreas = _requestService.GetAsync<List<Area>>("Areas");
            //var reqStockItemCategories = _requestService.GetAsync<List<StockItemCategory>>("StockItemCategories");
            var reqStockItems = _requestService.GetAsync<List<StockItem>>("StockItems");
            var reqBlobs = _blobStorageService.GetBlobs<CloudBlockBlob>("stockitemspng");
            await Task.WhenAll(reqApplicationUsers,
                               reqBars, 
                               reqBuildings,
                               reqAreas,
                               //reqStockItemCategories,
                               reqBlobs,
                               reqStockItems);
            _applicationUserContracts = await reqApplicationUsers;
            _bars = await reqBars;
            _buildings = await reqBuildings;
            _areas = await reqAreas;
            //_stockItemCategories = await reqStockItemCategories;
            _stockItems = await reqStockItems;
            _blobs = await reqBlobs;

            if(_currentArea == null 
               && _areas.Count > 0
               && App.CurrentApplicationUserContract == null 
               && _applicationUserContracts.Count > 0)
            {
                _currentArea = _areas[0];
                App.CurrentApplicationUserContract = _applicationUserContracts[0];
            }

            if (_stockItems.Count > 0)
            {
                SelectedStockItem = _stockItems[0];
                SelectedImageSource = null;
                if (!String.IsNullOrEmpty(_stockItems[0].ImagePath))
                {
                    var blob = _blobs.Find(p => p.Name.Equals(_stockItems[0].ImagePath));
                    if (blob != null)
                    {
                        SelectedImageSource = ImageSource.FromUri(blob.Uri);
                    }
                }
                _stockItemImages = new ImageSource[_stockItems.Count];
                for(int i = 0; i < _stockItems.Count; ++i)
                {
                    if(string.IsNullOrEmpty(_stockItems[i].ImagePath))
                    {
                        _stockItemImages[i] = null;
                        continue;
                    }
                    var b = _blobs.Find(p => p.Name.Equals(_stockItems[i].ImagePath));
                    _stockItemImages[i] = ImageSource.FromUri(b.Uri);
                }
            }
            StartedDateTime = DateTime.Now;
            Device.StartTimer(TimeSpan.FromSeconds(10), OnTimer);
            OnTimer();
        }
    }
}
