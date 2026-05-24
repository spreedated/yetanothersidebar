using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public abstract class Service<TServiceResponse> : IService<TServiceResponse>, IDisposable where TServiceResponse : class
    {
        private ILogger logger;
        private Timer runTimer;
        private bool disposedValue;

        public bool IsActive { get; internal set; } = true;
        public bool IsRunning { get; internal set; }
        public bool ProcessIsRunning { get; internal set; }
        public TServiceResponse Response { get; internal set; }
        public TimeSpan Interval { get; internal set; }

        public event EventHandler ProcessStarted;
        public event EventHandler DataUpdated;
        public event EventHandler<Exception> Error;

        /// <summary>
        /// Constructs a new Service
        /// </summary>
        /// <param name="interval">If no interval is provided, the timer is disabled</param>
        protected Service(TimeSpan interval = default, ILogger logger = null)
        {
            this.Response = Activator.CreateInstance<TServiceResponse>();
            this.logger = logger;

            if (interval != default)
            {
                this.CreateTimer(interval);
            }
        }

        internal abstract void Processor(object sender, ElapsedEventArgs e);

        internal void RaiseError(Exception ex)
        {
            this.Error?.Invoke(this, ex);
        }

        internal void RaiseProcessStarted()
        {
            this.ProcessStarted?.Invoke(this, EventArgs.Empty);
        }

        internal void RaiseValuesUpdated()
        {
            this.DataUpdated?.Invoke(this, EventArgs.Empty);
        }

        public async Task Run()
        {
            await Task.Factory.StartNew(() =>
            {
                this.Processor(this, null);
            });
        }

        public void Start()
        {
            if (!this.IsRunning && this.runTimer != null)
            {
                this.IsRunning = true;
                this.runTimer.Enabled = true;
                this.runTimer.Start();
            }
        }

        public void Stop()
        {
            if (this.IsRunning && this.runTimer != null)
            {
                this.IsRunning = false;
                this.runTimer.Enabled = false;
                this.runTimer.Stop();
            }
        }

        internal void CreateTimer(TimeSpan interval)
        {
            this.Interval = interval;
            this.runTimer = new()
            {
                Interval = this.Interval.TotalMilliseconds,
                Enabled = true
            };
            this.runTimer.Elapsed += this.Processor;
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.runTimer?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
