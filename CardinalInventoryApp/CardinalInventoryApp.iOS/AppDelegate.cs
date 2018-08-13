using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HealthKit;
using UIKit;

namespace CardinalInventoryApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        //private HKHealthStore healthKitStore = new HKHealthStore();

        //public override void OnActivated(UIApplication uiApplication)
        //{
        //    base.OnActivated(uiApplication);
        //    RequestAccessToHealthKit();
        //}

        //private void RequestAccessToHealthKit()
        //{
        //    var types = new NSSet(HKObjectType.GetWorkoutType(),
        //                          HKSeriesType.WorkoutRouteType,
        //                          HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned),
        //                          HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning),
        //                          HKQuantityType.Create(HKQuantityTypeIdentifier.DietaryWater));

        //    healthKitStore.RequestAuthorizationToShare(types, types, (isSuccess, error) =>
        //    {
        //        if (!isSuccess)
        //        {
        //            Console.WriteLine(error?.LocalizedDescription ?? "");
        //        }
        //    });
        //}

        /*
        private HKHealthStore healthKitStore = new HKHealthStore();

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            ValidateAuthorization();
        }

        private void ValidateAuthorization()
        {
            //var heartRateId = HKQuantityTypeIdentifierKey.HeartRate;
            //var heartRateType = HKObjectType.GetQuantityType(heartRateId);
            var heartRateType = HKQuantityType.Create(HKQuantityTypeIdentifier.DietaryWater);
            var typesToWrite = new NSSet(new[] { heartRateType });
            var typesToRead = new NSSet();
            healthKitStore.RequestAuthorizationToShare(
                    typesToWrite,
                    typesToRead,
                    ReactToHealthCarePermissions);
        }

        void ReactToHealthCarePermissions(bool success, NSError error)
        {
            //var access = healthKitStore.GetAuthorizationStatus(HKObjectType.GetQuantityType(HKQuantityTypeIdentifierKey.HeartRate));
            var access = healthKitStore.GetAuthorizationStatus(HKQuantityType.Create(HKQuantityTypeIdentifier.DietaryWater));
            if (access.HasFlag(HKAuthorizationStatus.SharingAuthorized))
            {
                HeartRateModel.Instance.Enabled = true;
            }
            else
            {
                HeartRateModel.Instance.Enabled = false;
            }
        }
        */
    }
}
