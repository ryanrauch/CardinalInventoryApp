using CardinalInventoryApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CardinalInventoryApp.ViewModels
{
    public class ReceiveItemViewModel : ViewModelBase
    {
        public override Task OnAppearingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
