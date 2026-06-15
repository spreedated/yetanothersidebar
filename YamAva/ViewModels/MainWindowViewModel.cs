using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using neXn.Ui.Avalonia;
using Services.Models;
using Services.Workers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using YamAva.Logic;
using YamAva.ViewElements;
using YamAva.Views;

namespace YamAva.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private const string VERSION_OLD_BRUSH_KEY = "VersionOldBrush";
        private const string VERSION_CURRENT_BRUSH_KEY = "VersionCurrentBrush";
        private const string VERSION_NEWER_BRUSH_KEY = "VersionNewerBrush";

        private readonly System.Timers.Timer _weatherLoadingSpinnerTimer;
        private int _weatherLoadingSpinnerTicks = 5;

        [ObservableProperty]
        public partial string AppTitle { get; set; }

        [ObservableProperty]
        public partial string AppVersion { get; set; }

        [ObservableProperty]
        public partial string Title { get; set; }

        [ObservableProperty]
        public partial string Status { get; set; }

        [ObservableProperty]
        public partial bool TopMostEnabled { get; set; }

        [ObservableProperty]
        public partial Version Blender { get; set; }
        partial void OnBlenderChanged(Version value)
        {
            this.CompareToInstalledSoftware();
        }

        [ObservableProperty]
        public partial SolidColorBrush BlenderVersionColor { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);

        [ObservableProperty]
        public partial Version Godot { get; set; }
        partial void OnGodotChanged(Version value)
        {
            this.CompareToInstalledSoftware();
        }

        [ObservableProperty]
        public partial SolidColorBrush GodotVersionColor { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);

        #region Ctor
        public MainWindowViewModel() : base("MainWindowViewModel")
        {
            this.AppTitle = Globals.Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            this.AppVersion = $"v{Globals.Assembly.GetName().Version}";
            this.Title = $"{this.AppTitle} {this.AppVersion}";

            Random rnd = new();
            this.LgsLoadingIndicatorMode = (LoadingIndicatorMode)rnd.Next(0, Enum.GetValues<LoadingIndicatorMode>().Length);

            if (Design.IsDesignMode)
            {
                this.WeatherResponse = new()
                {
                    WeatherCurrent = new()
                    {
                        TempC = 20,
                        Condition = new()
                        {
                            Text = "Sunny",
                            Icon = "https://cdn.weatherapi.com/weather/64x64/day/113.png",
                            Code = 1000
                        },
                        AirQuality = new()
                        {
                            GbDefraIndex = 1,
                            UsEpaIndex = 1,
                            So2 = 1,
                            No2 = 1,
                            O3 = 1,
                            Pm10 = 1,
                            Co = 1,
                            Pm25 = 1
                        },
                        Cloud = 20,
                        FeelslikeC = 19,
                        Humidity = 50,
                        WindKph = 10,
                        WindDir = "NE",
                        WindDegree = 45,
                        IsDay = true,
                        Uv = 5,
                        VisKm = 6,
                        GustKph = 7,
                        WindMph = 8,
                        LastUpdated = DateTime.Now,
                        LastUpdatedEpoch = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(),
                        PrecipIn = 0,
                        PrecipMm = 0,
                        PressureIn = 30,
                        PressureMb = 1016
                    }
                };

                this.FpvSoftwareVersions = new()
                {
                    BetaflightFw = new Version(4, 3, 0),
                    BlueJay = new Version(1, 2, 3),
                    EdgeTX = new Version(3, 4, 5),
                    ExpressLRS = new Version(2, 3, 4),
                    Whoopstor3 = new Version(1, 0, 0)
                };

                this.Blender = new(1, 1, 1);
                this.Godot = new(1, 1, 1);

                this.BlenderVersionColor = GetBrush(VERSION_CURRENT_BRUSH_KEY);
                this.GodotVersionColor = GetBrush(VERSION_NEWER_BRUSH_KEY);
            }

            if (!Design.IsDesignMode)
            {
                _weatherLoadingSpinnerTimer = new()
                {
                    Interval = 1000
                };

                _weatherLoadingSpinnerTimer.Elapsed += (s, v) =>
                {
                    _weatherLoadingSpinnerTicks--;

                    if (_weatherLoadingSpinnerTicks <= 0)
                    {
                        Application.Current.Dispatcher.Post(() => this.IsWeatherLoading = true);
                        _weatherLoadingSpinnerTicks = 5;
                        _weatherLoadingSpinnerTimer.Stop();
                    }
                };

                this.TopMostEnabled = Globals.UserConfig.RuntimeConfiguration.TopMost;

                Globals.BackgroundServices.Services.GetService<BlenderWorker>().LatestVersionUpdated += (s, v) =>
                {
                    this.Blender = v;
                };

                Globals.BackgroundServices.Services.GetService<GodotWorker>().LatestVersionUpdated += (s, v) =>
                {
                    this.Godot = v;
                };

                Globals.BackgroundServices.Services.GetService<InstalledSoftwareWorker>().InstalledSoftwareUpdated += (s, v) =>
                {
                    this.InstalledSoftware = v;
                };

                Globals.BackgroundServices.Services.GetService<LgDeviceWorker>().LatestVersionsUpdated += (s, v) =>
                {
                    this.LogitechDevices = new ObservableCollection<LogitechDevice>(v);

                    Task.Run(async () =>
                    {
                        await Task.Delay(2500);
                        this.LgsDataUpdated();
                    });
                };

                Globals.BackgroundServices.Services.GetService<WeatherWorker>().LatestWeatherUpdated += (s, v) =>
                {
                    this.WeatherResponse = v;
                };

                Globals.BackgroundServices.Services.GetService<WeatherWorker>().ProcessingStarted += (s, v) =>
                {
                    if (_weatherLoadingSpinnerTimer.Enabled)
                    {
                        return;
                    }

                    _weatherLoadingSpinnerTimer.Start();
                };

                Globals.BackgroundServices.Services.GetService<WeatherWorker>().ProcessingFinished += (s, v) =>
                {
                    this.IsWeatherLoading = false;

                    _weatherLoadingSpinnerTimer.Stop();
                    _weatherLoadingSpinnerTicks = 5;
                };

                Globals.BackgroundServices.Services.GetService<FpvSoftwareWorker>().LatestVersionsUpdated += (s, v) =>
                {
                    this.FpvSoftwareVersions = v;
                };

                Globals.BackgroundServices.Services.GetService<UniFiWorker>().LatestUnifiUpdated += (s, v) =>
                {
                    this.UnifiData = v;
                };

                // Start background services
                Task.Run(() =>
                {
                    foreach (var s in Globals.BackgroundServices.Services.GetServices<IHostedService>())
                    {
                        Task.Run(async () =>
                        {
                            await s.StartAsync(CancellationToken.None);
                            this.LogServiceStarted(s.GetType().FullName);
                        });
                    }
                });
            }
        }
        #endregion

        private static SolidColorBrush GetVersionColor(Version latest, Version installed)
        {
            if (latest == null || installed == null)
            {
                return GetBrush(VERSION_CURRENT_BRUSH_KEY);
            }

            if (latest < installed)
            {
                return GetBrush(VERSION_NEWER_BRUSH_KEY);
            }

            if (latest == installed)
            {
                return GetBrush(VERSION_CURRENT_BRUSH_KEY);
            }

            return GetBrush(VERSION_OLD_BRUSH_KEY);
        }

        private static SolidColorBrush GetBrush(string key)
        {
            if (Application.Current?.TryFindResource(key, out var resource) == true && resource is SolidColorBrush brush)
            {
                return brush;
            }

            // Fallback to a default brush if resource not found
            return new SolidColorBrush(Colors.WhiteSmoke);
        }

        [RelayCommand]
        private void CloseApplication()
        {
            this.Instance.Close();
        }

        [RelayCommand]
        private static void OpenConfigFolder()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Globals.AppLocalBaseUserPath,
                UseShellExecute = true
            });
        }

        [RelayCommand]
        private void ToggleTopMost()
        {
            Globals.UserConfig.RuntimeConfiguration.TopMost ^= true;
            this.Instance.Topmost = Globals.UserConfig.RuntimeConfiguration.TopMost;
            this.TopMostEnabled = Globals.UserConfig.RuntimeConfiguration.TopMost;
            Task.Run(async () => await Globals.UserConfig.SaveAsync());
        }

        #region Logitech Devices
        [ObservableProperty]
        public partial ObservableCollection<LogitechDevice> LogitechDevices { get; set; }

        [ObservableProperty]
        public partial LoadingIndicatorMode LgsLoadingIndicatorMode { get; set; }

        private void LgsDataUpdated()
        {
            if (this.Instance == null)
            {
                while (this.Instance == null || this.LogitechDevices == null || this.LogitechDevices.Count <= 0)
                {
                    Thread.Sleep(100);
                }
            }

            this.Instance.Dispatcher.Invoke(() =>
            {
                StackPanel p = ((MainWindow)this.Instance).LgsStackPanel;

                if (p.Children.Count != this.LogitechDevices.Count)
                {
                    p.Children.Clear();

                    foreach (LogitechDevice d in this.LogitechDevices)
                    {
                        LgsDeviceElement lde = new()
                        {
                            LogitechDevice = d
                        };

                        p.Children.Add(lde);
                    }

                    return;
                }

                foreach (LogitechDevice d in this.LogitechDevices)
                {
                    LgsDeviceElement k = p.Children.OfType<LgsDeviceElement>().FirstOrDefault(x => x.LogitechDevice.DeviceId == d.DeviceId);

                    if (k == null)
                    {
                        LgsDeviceElement lde = new()
                        {
                            LogitechDevice = d
                        };
                        p.Children.Add(lde);
                    }
                    else
                    {
                        k.LogitechDevice = d;
                    }
                }
            });
        }
        [RelayCommand]
        private static void RefreshLgDevices()
        {
            Task.Run(async () =>
            {
                await Globals.BackgroundServices.Services.GetServices<IHostedService>().OfType<IServiceWorker>().First(x => x.GetType() == typeof(LgDeviceWorker)).Process();
            });
        }
        #endregion

        #region Weather
        [ObservableProperty]
        public partial WeatherApi WeatherResponse { get; set; }

        [ObservableProperty]
        public partial bool IsWeatherLoading { get; set; }

        [RelayCommand]
        private static void RefreshWeather()
        {
            Task.Run(async () =>
            {
                await Globals.BackgroundServices.Services.GetServices<IHostedService>().OfType<IServiceWorker>().First(x => x.GetType() == typeof(WeatherWorker)).Process();
            });
        }
        #endregion

        #region FpvFirmwares
        [ObservableProperty]
        public partial FpvSoftwareVersions FpvSoftwareVersions { get; set; }
        partial void OnFpvSoftwareVersionsChanged(FpvSoftwareVersions value)
        {
            this.CompareToInstalledSoftware();
        }
        #endregion

        #region Installed Software List
        [ObservableProperty]
        public partial InstalledSoftware InstalledSoftware { get; set; }
        partial void OnInstalledSoftwareChanged(InstalledSoftware value)
        {
            this.CompareToInstalledSoftware();
        }

        [ObservableProperty]
        public partial SolidColorBrush BluejayVersionColor { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);

        [ObservableProperty]
        public partial SolidColorBrush BetaflightVersionColor { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);

        [ObservableProperty]
        public partial SolidColorBrush ElrsVersionColor { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);

        [ObservableProperty]
        public partial SolidColorBrush EdgeTxVersionColor { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);

        [ObservableProperty]
        public partial SolidColorBrush WhoopStorVersionColor { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);

        private void CompareToInstalledSoftware()
        {
            Application.Current.Dispatcher.Post(() =>
            {
                if (this.InstalledSoftware != null)
                {
                    if (this.FpvSoftwareVersions != null)
                    {
                        this.BluejayVersionColor = GetVersionColor(this.FpvSoftwareVersions.BlueJay, this.InstalledSoftware.Bluejay);
                        this.BetaflightVersionColor = GetVersionColor(this.FpvSoftwareVersions.BetaflightFw, this.InstalledSoftware.BetaflightFw);
                        this.ElrsVersionColor = GetVersionColor(this.FpvSoftwareVersions.ExpressLRS, this.InstalledSoftware.ExpressLrs);
                        this.EdgeTxVersionColor = GetVersionColor(this.FpvSoftwareVersions.EdgeTX, this.InstalledSoftware.EdgeTx);
                        this.WhoopStorVersionColor = GetVersionColor(this.FpvSoftwareVersions.Whoopstor3, this.InstalledSoftware.Whoopstor);
                    }

                    this.BlenderVersionColor = GetVersionColor(this.Blender, this.InstalledSoftware.Blender);
                    this.GodotVersionColor = GetVersionColor(this.Godot, this.InstalledSoftware.Godot);
                }
            });
        }
        #endregion

        #region Unifi
        [ObservableProperty]
        public partial UnifiData UnifiData { get; set; }
        #endregion

        #region Logging
        [LoggerMessage(Level = LogLevel.Trace, Message = "Started background service {ServiceName}")]
        private partial void LogServiceStarted(string serviceName);
        #endregion
    }
}
