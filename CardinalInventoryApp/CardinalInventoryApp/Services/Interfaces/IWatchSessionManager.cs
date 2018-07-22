using System;
using System.Collections.Generic;

namespace CardinalInventoryApp.Services.Interfaces
{
    public interface IWatchSessionManager<T>
    {
        event EventHandler<T> MessageReceived;
        bool IsPairedSession();
        bool IsReachableSession();
        void StartSession();
        void SendMessage(T msg);
    }
}