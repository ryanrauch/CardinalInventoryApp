using CardinalInventoryApp.Services.Interfaces;
using CardinalInventoryApp.UWP.DependencyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(StubWatchSessionManager))]
namespace CardinalInventoryApp.UWP.DependencyServices
{
    public class StubWatchSessionManager : IWatchSessionManager
    {
        public event EventHandler<WatchDataEventArgs> DataReceived;

        public bool IsPairedSession()
        {
            throw new NotImplementedException();
        }

        public bool IsReachableSession()
        {
            throw new NotImplementedException();
        }

        public void SendData(WatchDataType type, string data)
        {
            throw new NotImplementedException();
        }

        public void SendData(WatchDataType type, double x, double y, double z)
        {
            throw new NotImplementedException();
        }

        public void StartSession()
        {
            throw new NotImplementedException();
        }

        public void StopSession()
        {
            throw new NotImplementedException();
        }
    }
}
