using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Workers
{
    public class InstalledSoftwareWorker : BackgroundService
    {
        private const string SOFTAREPACK_FILENNAME = "softwarepack.json";
        private readonly FileSystemWatcher _watcher;
        private readonly string _configPath;
        private readonly ILogger _logger;

        public InstalledSoftware InstalledSoftware { get; private set; }

        public event EventHandler<InstalledSoftware> InstalledSoftwareUpdated;

        #region Ctor
        public InstalledSoftwareWorker(ILogger logger, string configPath)
        {
            _logger = logger;
            _configPath = configPath;

            _watcher = new(_configPath, "*.json")
            {
                EnableRaisingEvents = true
            };

            _watcher.Changed += async (s, e) =>
            {
                if (!e.Name.Contains(SOFTAREPACK_FILENNAME))
                {
                    return;
                }

                await this.UpdateInstalledSoftware();
            };
        }
        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await this.UpdateInstalledSoftware();

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _watcher.Dispose();
            await base.StopAsync(cancellationToken);
        }

        private async Task UpdateInstalledSoftware()
        {
            if (!File.Exists(Path.Combine(_configPath, SOFTAREPACK_FILENNAME)))
            {
                return;
            }

            using (FileStream fs = File.Open(Path.Combine(_configPath, SOFTAREPACK_FILENNAME), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.InstalledSoftware = await JsonSerializer.DeserializeAsync<InstalledSoftware>(fs);
            }

            this.InstalledSoftwareUpdated?.Invoke(this, this.InstalledSoftware);
            _logger?.LogTrace("Installed software updated.");
        }
    }
}
