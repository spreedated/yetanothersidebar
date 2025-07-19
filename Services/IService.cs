using System;
using System.Threading.Tasks;

namespace Services
{
    interface IService<out TResponseModel> where TResponseModel : class
    {
        public bool IsActive { get; }
        public bool IsRunning { get; }
        public bool ProcessIsRunning { get; }
        public TResponseModel Response { get; }
        public TimeSpan Interval { get; }

        public event EventHandler ProcessStarted;
        public event EventHandler DataUpdated;
        public event EventHandler<Exception> Error;

        public void Start();
        public void Stop();
        public Task Run();
    }
}
