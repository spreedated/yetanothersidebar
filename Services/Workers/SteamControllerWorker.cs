#pragma warning disable CA1416 // Suppress "Platform compatibility" warning since this code is intended to run on Windows and uses Windows-specific APIs

using Microsoft.Extensions.Logging;
using SDL3;
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
    public class SteamControllerWorker : ServiceWorker
    {
        private readonly ILogger _logger;
        private nint _gamepad = nint.Zero;
        private bool _sdlInitialized;

        public int Retries { get; set; } = 50;
        public int DelayBeforeRetry { get; set; } = 250;

        public SteamController Device { get; private set; }

        public event EventHandler<SteamController> StateUpdated;

        public SteamControllerWorker(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!this.InitializeSdl())
            {
                _logger?.LogError("SDL3 critical error");
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

        private bool InitializeSdl()
        {
            if (_sdlInitialized)
            {
                return true;
            }

            if (!SDL.Init(SDL.InitFlags.Gamepad))
            {
                _logger.LogError("SDL3 couldn't initialize: {Error}", SDL.GetError());
                return false;
            }

            _sdlInitialized = true;

            return true;
        }

        [SupportedOSPlatform("windows")]
        private async Task<SteamController> GetData()
        {
            SDL.PumpEvents();
            SDL.UpdateGamepads();

            int _retries = this.Retries;

            if (_gamepad != nint.Zero && !SDL.GamepadConnected(_gamepad))
            {
                SDL.CloseGamepad(_gamepad);
                _gamepad = nint.Zero;
            }

            if (_gamepad == nint.Zero)
            {
                uint[] gamepads = SDL.GetGamepads(out int count);

                if (count == 0)
                {
                    return new();
                }

                uint? steamControllerId = null;

                foreach (uint id in gamepads)
                {
                    ushort vendor = SDL.GetGamepadVendorForID(id);
                    ushort product = SDL.GetGamepadProductForID(id);

                    if (vendor != 0x28DE)
                    {
                        continue;
                    }

                    // 1302 = direct USB
                    // 1304 = wireless puck
                    if (product is 0x1302 or 0x1304)
                    {
                        steamControllerId = id;
                        break;
                    }
                }

                if (steamControllerId is null)
                {
                    return new();
                }

                _gamepad = SDL.OpenGamepad(steamControllerId.Value);

                if (_gamepad == nint.Zero)
                {
                    _logger.LogError("Couldn't open Steam Controller: {Error}", SDL.GetError());

                    return new();
                }

                _logger.LogInformation("Steam Controller connected: {Name}", SDL.GetGamepadName(_gamepad));
            }

            SDL.PowerState state = SDL.GetGamepadPowerInfo(_gamepad, out int percentage);

            while (_retries > 0 && state == SDL.PowerState.Unknown)
            {
                SDL.PumpEvents();
                SDL.UpdateGamepads();
                state = SDL.GetGamepadPowerInfo(_gamepad, out percentage);

                if (state == SDL.PowerState.Unknown)
                {
                    _retries--;
                    await Task.Delay(this.DelayBeforeRetry);
                    continue;
                }
            }

            return new SteamController
            {
                BatteryPercentage = percentage,
                Powerstate = Enum.Parse<SteamControllerPowerState>(state.ToString())
            };
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
