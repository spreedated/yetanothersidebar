using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Workers
{
    public class AudioWorker : ServiceWorker
    {
        private readonly ILogger _logger;
        private MMDevice _device;

        public event EventHandler<float> VolumeChanged;

        public float Volume
        {
            get => _device.AudioEndpointVolume.MasterVolumeLevelScalar;
            set => _device.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Clamp(value, 0f, 1f);
        }

        public bool IsMuted
        {
            get => _device.AudioEndpointVolume.Mute;
            set => _device.AudioEndpointVolume.Mute = value;
        }

        #region Ctor
        public AudioWorker(ILogger logger)
        {
            _logger = logger;

            MMDeviceEnumerator enumerator = new();
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            _device.AudioEndpointVolume.OnVolumeNotification += this.OnVolumeNotification;
        }
        #endregion

        private void OnVolumeNotification(AudioVolumeNotificationData data)
        {
            this.VolumeChanged?.Invoke(this, data.MasterVolume);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        public override async Task Process()
        {
            //noop
        }
    }
}
