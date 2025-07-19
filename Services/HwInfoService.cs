using Services.Models.Responses;
using System;
using System.Linq;
using System.Timers;

namespace Services
{
    public sealed class HwInfoService : Service<HwInfoResponse>
    {
        private readonly HwHash.HwHash hwHash;

        public HwInfoService()
        {
            base.CreateTimer(new TimeSpan(0, 10, 0));

            this.hwHash = new();

            this.hwHash.SensorsUpdated += (s, e) =>
            {
                base.Response ??= new();
                base.Response.Telemetric = this.hwHash.GetRelevantList().ToList();

                base.RaiseValuesUpdated();
            };
        }

        internal override void Processor(object sender, ElapsedEventArgs e)
        {
            this.ProcessIsRunning = true;
            base.RaiseProcessStarted();

            try
            {
                if (!this.hwHash.IsRunning)
                {
                    this.hwHash.Start();
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
