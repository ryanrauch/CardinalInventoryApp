using System;
using System.Threading.Tasks;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels.Base;

namespace CardinalInventoryApp.ViewModels
{
    public class ChartViewModel : ViewModelBase
    {
        private readonly IRequestService _requestService;

        public ChartViewModel(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public override Task OnAppearingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
