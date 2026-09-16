#pragma warning disable CA1416 // Suppress "Platform compatibility" warning since this code is intended to run on Windows and uses Windows-specific APIs

using Microsoft.Extensions.Logging;
using neXn.SteamController2026.SDL3;
using neXn.SteamController2026.SDL3.Models;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Workers
{
    public class SteamControllerWorker : ServiceWorker
    {
        private readonly ILogger _logger;
        private readonly SteamControllerClient _steamControllerClient;

        public SteamControllerStatus Device { get; private set; }
        public event EventHandler<SteamControllerStatus> StateUpdated;

        public SteamControllerWorker(ILogger logger)
        {
            _logger = logger;
            _steamControllerClient = new(_logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_steamControllerClient.Initialize())
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!base.IsEnabled)
                {
                    _logger?.LogTrace("SteamControllerWorker is disabled. Skipping processing.");
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
        private async Task<SteamControllerStatus> GetData()
        {
            return await _steamControllerClient.GetStatusAsync();
        }

        public override async Task Process()
        {
            _logger.LogTrace("SteamControllerWorker is running.");
            base.RaiseProcessedStarted();
            Stopwatch s = Stopwatch.StartNew();

            this.Device = await this.GetData();
            this.StateUpdated?.Invoke(this, this.Device);

            s.Stop();
            base.RaiseProcessedFinished();
            _logger.LogTrace("SteamControllerWorker completed a cycle in {ElapsedMilliseconds} ms.", s.ElapsedMilliseconds);
        }
    }
}
