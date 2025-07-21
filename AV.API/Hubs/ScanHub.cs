using AV.Engine.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AV.API.Hubs
{
    public class ScanHub : Hub
    {
        private readonly IAVEngine _engine;
        private static bool _connected;

        public ScanHub(IAVEngine engine)
        {
            _engine = engine;
        }

        public override Task OnConnectedAsync()
        {
            if (_connected)
                throw new HubException("Only one client allowed");

            //_connected = true;
            //_engine.ScanStarted += OnStarted;
            //_engine.ThreatsFound += OnThreats;
            //_engine.ScanStopped += OnStopped;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //_engine.ScanStarted -= OnStarted;
            //_engine.ThreatsFound -= OnThreats;
            //_engine.ScanStopped -= OnStopped;
            //_connected = false;
            return base.OnDisconnectedAsync(exception);
        }

        //private void OnStarted(object _, ScanStartedEvent e)
        //{
        //    Clients.Caller.SendAsync("ScanStarted", e);
        //}

        //private void OnThreats(object _, ThreatsFound e)
        //{
        //    Clients.Caller.SendAsync("ThreatsFound", e);
        //}

        //private void OnStopped(object _, ScanStoppedEvent e)
        //{
        //    Clients.Caller.SendAsync("ScanStopped", e);
        //}

        //private Task OnStarted(object _, ScanStartedEvent e)
        //    => Clients.Caller.SendAsync("ScanStarted", e);

        //private Task OnThreats(object _, ThreatsFoundEvent e)
        //    => Clients.Caller.SendAsync("ThreatsFound", e);

        //private Task OnStopped(object _, ScanStoppedEvent e)
        //    => Clients.Caller.SendAsync("ScanStopped", e);
    }
}
