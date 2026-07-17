#pragma warning disable CA1416 // Suppress "Platform compatibility" warning since this code is intended to run on Windows and uses Windows-specific APIs

using Microsoft.Extensions.Logging;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Workers
{
    public class LgDeviceWorker : ServiceWorker
    {
        private readonly ILogger _logger;
        public LogitechDevice[] Devices { get; private set; }

        public event EventHandler<LogitechDevice[]> LatestVersionsUpdated;

        public LgDeviceWorker(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!base.IsEnabled)
                {
                    _logger?.LogTrace("LgDeviceWorker is disabled. Skipping processing.");
                    return;
                }

                try
                {
                    await this.Process();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Process error");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        [SupportedOSPlatform("windows")]
        private async Task<LogitechDevice[]> GetData()
        {
            string json = null;
            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting($"Global\\LGS_Devices"))
                {
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        ushort length = accessor.ReadUInt16(0);
                        byte[] buffer = new byte[length];
                        accessor.ReadArray(2, buffer, 0, length);
                        json = System.Text.Encoding.UTF8.GetString(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while retrieving Logitech device data.");
                return null;
            }

            return [.. JsonSerializer.Deserialize<IEnumerable<LogitechDevice>>(json)];
        }

        public override async Task Process()
        {
            _logger.LogTrace("LgDeviceWorker is running.");
            base.RaiseProcessedStarted();
            Stopwatch s = Stopwatch.StartNew();

            this.Devices = await this.GetData();
            this.LatestVersionsUpdated?.Invoke(this, this.Devices);

            s.Stop();
            base.RaiseProcessedFinished();
            _logger.LogTrace("LgDeviceWorker completed a cycle in {ElapsedMilliseconds} ms.", s.ElapsedMilliseconds);
        }
    }
}
