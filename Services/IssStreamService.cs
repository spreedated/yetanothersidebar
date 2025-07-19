using com.lightstreamer.client;
using Services.Models;
using Services.Models.Responses;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public sealed class IssStreamService : Service<IssResponse>
    {
        private readonly LightstreamerClient client;
        private readonly IssListener issListener;

        public IssStreamService()
        {
            base.CreateTimer(new TimeSpan(0, 10, 0));

            this.client ??= new("https://push.lightstreamer.com", "ISSLIVE");
            this.issListener ??= new();

            this.client.subscribe(new Subscription("MERGE", "NODE3000005", ["Value", "Status"]));
            this.client.subscribe(new Subscription("MERGE", "NODE3000008", ["Value", "Status"]));
            this.client.subscribe(new Subscription("MERGE", "NODE3000009", ["Value", "Status"]));

            this.client.Subscriptions[0].addListener(this.issListener);
            this.client.Subscriptions[1].addListener(this.issListener);
            this.client.Subscriptions[2].addListener(this.issListener);

            this.issListener.ValuesUpdated += (s, e) =>
            {
                base.Response ??= new();
                base.Response = this.issListener.IssResponse;

                base.RaiseValuesUpdated();
            };
        }

        internal override void Processor(object sender, ElapsedEventArgs e)
        {
            this.ProcessIsRunning = true;
            base.RaiseProcessStarted();

            try
            {
                if (!this.client.Status.StartsWith("CONNECTED", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.client.connect();
                }

                while (!this.client.Status.StartsWith("CONNECTED", StringComparison.InvariantCultureIgnoreCase))
                {
                    Task.Delay(100).Wait();
                }

                while (this.issListener.Updated == default)
                {
                    Task.Delay(500).Wait();
                }
            }
            catch (Exception ex)
            {
                base.RaiseError(ex);
            }

            this.ProcessIsRunning = false;
        }
    }
}
