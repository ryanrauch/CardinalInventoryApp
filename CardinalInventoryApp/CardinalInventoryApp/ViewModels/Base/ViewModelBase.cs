using System.Threading.Tasks;

namespace CardinalInventoryApp.ViewModels.Base
{
    public abstract class ViewModelBase : ExtendedBindableObject
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        private object _initializeParameter { get; set; }
        public object InitializeParameter
        {
            get { return _initializeParameter; }
            set
            {
                _initializeParameter = value;
                RaisePropertyChanged(() => InitializeParameter);
            }
        }

        public ViewModelBase() { }

        public virtual void Initialize(object param)
        {
            InitializeParameter = param;
        }

        public abstract Task OnAppearingAsync();
    }
}
