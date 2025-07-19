#pragma warning disable S112

using NAudio.CoreAudioApi;
using Serilog;
using Services.Models.Responses;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace Services
{
    public class MicrophoneService : Service<MicrophoneResponse>
    {
        private ILogger logger;
        private bool isPolling;
        private CancellationTokenSource cts;

        #region Constructor
        public MicrophoneService(ILogger logger = null) : base()
        {
            this.logger = logger;
        }
        #endregion

        internal override void Processor(object sender, ElapsedEventArgs e)
        {
            this.cts = new();
            this.ProcessIsRunning = true;
            base.RaiseProcessStarted();

            Task.Factory.StartNew(() =>
            {
                while (!this.cts.IsCancellationRequested)
                {
                    this.Poll();
                    base.RaiseValuesUpdated();
                    Thread.Sleep(50);
                }

                this.ProcessIsRunning = false;
            });
        }

        public new void Stop()
        {
            base.Stop();
            this.cts?.Cancel();
        }

        internal void Poll()
        {
            if (this.isPolling)
            {
                return;
            }

            this.isPolling = true;

            try
            {
                MMDeviceEnumerator mm = new();

                MMDeviceCollection devices = mm.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                MMDevice primaryDevice = devices.FirstOrDefault(x => x.FriendlyName.Contains("g522", StringComparison.CurrentCultureIgnoreCase));

                if (primaryDevice == null)
                {
                    throw new Exception(nameof(primaryDevice));
                }

                base.Response = new()
                {
                    IsMuted = primaryDevice.AudioEndpointVolume.Mute,
                    InputValue = primaryDevice.AudioMeterInformation.MasterPeakValue * 100
                };
            }
            catch (Exception ex)
            {
                Log.Error("Micrphone polling error: ", ex);
            }

            this.isPolling = false;
        }
    }
}
