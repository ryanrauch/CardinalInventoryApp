using Autofac;
using CardinalInventoryApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CardinalInventoryApp.Views.Base
{
    public class ViewPageBase<T> : ContentPage where T : ViewModelBase
    {
        private readonly T _viewModel;
        public T ViewModel
        {
            get { return _viewModel; }
        }

        public ViewPageBase()
        {
            using (var scope = App.Container.BeginLifetimeScope())
            {
                _viewModel = App.Container.Resolve<T>();
            }
            BindingContext = _viewModel;
            AttachEventHandlers();
        }

        //public async Task InitializeViewModelAsync()
        //{
        //    await (BindingContext as ViewModelBase).InitializeAsync();
        //}

        private void AttachEventHandlers()
        {
            Appearing += async (sender, e) =>
            {
                if (BindingContext is ViewModelBase viewModelBase)
                {
                    await viewModelBase.OnAppearingAsync();
                }
            };

            //Disappearing += (sender, e) =>
            //{
            //    if (BindingContext is ViewModelBase viewModelBase)
            //    {
            //        viewModelBase.OnDisappearing();
            //    }
            //};
        }
    }
}
