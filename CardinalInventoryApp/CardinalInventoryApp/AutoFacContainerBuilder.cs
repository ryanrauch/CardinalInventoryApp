﻿using Autofac;
using CardinalInventoryApp.Services;
using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CardinalInventoryApp
{
    public static class AutoFacContainerBuilder
    {
        public static IContainer CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<InitialViewModel>().SingleInstance();
            containerBuilder.RegisterType<LoginViewModel>().SingleInstance();
            containerBuilder.RegisterType<InventoryViewModel>().SingleInstance();
            containerBuilder.RegisterType<InventoryCompletedViewModel>().SingleInstance();
            containerBuilder.RegisterType<ChartViewModel>().SingleInstance();
            containerBuilder.RegisterType<ScanBarcodeViewModel>().SingleInstance();
            containerBuilder.RegisterType<ReceiveItemViewModel>().SingleInstance();


            containerBuilder.RegisterType<UnAuthenticatedRequestService>().As<IRequestService>().SingleInstance();
            containerBuilder.RegisterType<BlobStorageService>().As<IBlobStorageService>().SingleInstance();
            containerBuilder.RegisterType<SinglePageNavigationService>().As<INavigationService>().SingleInstance();

            containerBuilder.RegisterInstance(DependencyService.Get<IWatchSessionManager>()).AsImplementedInterfaces().SingleInstance();

            return containerBuilder.Build();
        }
    }
}
