using CardinalInventoryApp.Contracts;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
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
        }

        private Area _currentArea { get; set; }

        private int _stockItemIndex = 0;
        private int _imagePreloadCount = 3;

        public ICommand NextStockItemCommand => new Command(async () => await NextStockItemTask());
        public ICommand SkipStockItemCommand => new Command(async () => await SkipStockItemTask());

        private String _statusMessage { get; set; } = String.Empty;
        public String StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                RaisePropertyChanged(() => StatusMessage);
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
                RaisePropertyChanged(() => SelectedStockItem);
                //UpdateSelectedStockItemImage(_selectedStockItem);
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

        private async Task<bool> CreateInventoryActionHistory()
        {
            var ah = new InventoryActionHistory()
            {
                InventoryActionHistoryId = Guid.NewGuid(),
                Action = InventoryAction.UserViewedAuto,
                StockItemId = SelectedStockItem.StockItemId,
                Timestamp = DateTime.Now,
                ItemLevel = SelectedItemLevel,
                ApplicationUserId = App.CurrentApplicationUserContract.Id,
                AreaId = _currentArea.AreaId
            };
            var res = await _requestService.PostAsync<InventoryActionHistory>("inventoryActionHistories", ah);
            return res != null;
        }

        private async Task SkipStockItemTask()
        {
            //if (!await CreateInventoryActionHistory())
            //{
            //    return;
            //}
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

        private async Task NextStockItemTask()
        {
            if(!await CreateInventoryActionHistory())
            {
                StatusMessage = "CreateInventoryActionHistory::Failed";
            }
            else
            {
                StatusMessage = "Accepted";
            }
            SelectedImageSource = _stockItemImages[_stockItemIndex];
            SelectedItemLevel = 1.0M;
            return; //reset quantity & continue for multiple of same stockitem's with different qty.

            if(++_stockItemIndex < _stockItems.Count)
            {
                SelectedStockItem = _stockItems[_stockItemIndex];
                SelectedImageSource = _stockItemImages[_stockItemIndex];
                if(_stockItemIndex > 0)
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

        /************/
        //Data-Repository
        private List<ApplicationUserContract> _applicationUserContracts { get; set; }
        private List<Bar> _bars { get; set; }
        private List<Building> _buildings { get; set; }
        private List<Area> _areas { get; set; }
        private List<StockItemCategory> _stockItemCategories { get; set; }
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
            var reqStockItemCategories = _requestService.GetAsync<List<StockItemCategory>>("StockItemCategories");
            var reqStockItems = _requestService.GetAsync<List<StockItem>>("StockItems");
            var reqBlobs = _blobStorageService.GetBlobs<CloudBlockBlob>("stockitemphotos");
            await Task.WhenAll(reqApplicationUsers,
                               reqBars, 
                               reqBuildings,
                               reqAreas,
                               reqStockItemCategories,
                               reqStockItems,
                               reqBlobs);
            _applicationUserContracts = await reqApplicationUsers;
            _bars = await reqBars;
            _buildings = await reqBuildings;
            _areas = await reqAreas;
            _stockItemCategories = await reqStockItemCategories;
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
                var blob = _blobs.Find(p => p.Name.Equals(_stockItems[0].ImagePath));
                if (blob != null)
                {
                    SelectedImageSource = ImageSource.FromUri(blob.Uri);
                }
                else
                {
                    SelectedImageSource = null;
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
        }
    }
}
