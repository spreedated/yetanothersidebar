using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Services.Workers
{
    public abstract class ServiceWorker : BackgroundService
    {
        public event EventHandler<EventArgs> ProcessingStarted;
        public event EventHandler<EventArgs> ProcessingFinished;

        public bool IsEnabled { get; private set; } = true;

        public void ToggleService()
        {
            this.IsEnabled ^= true;
        }

        public void RaiseProcessedStarted()
        {
            this.ProcessingStarted?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseProcessedFinished()
        {
            this.ProcessingFinished?.Invoke(this, EventArgs.Empty);
        }

        public abstract Task Process();
    }
}
