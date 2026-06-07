using System;
using System.Threading.Tasks;

namespace Services.Workers
{
    public interface IServiceWorker
    {
        public event EventHandler<EventArgs> ProcessingStarted;
        public event EventHandler<EventArgs> ProcessingFinished;

        public Task Process();
    }
}
