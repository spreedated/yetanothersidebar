using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using neXn.Ui.Avalonia;
using Services;
using Services.Models;
using Services.Models.Responses;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using YamAva.Logic;
using YamAva.Models;
using YamAva.ViewElements;
using YamAva.Views;

namespace YamAva.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
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

                this.SoftwareResponse = new()
                {
                    Godot = new Version(3, 5, 1),
                    Date = DateTime.Now
                };

                this.FpvResponse = new()
                {
                    BetaflightFw = new Version(4, 3, 0),
                    BlueJay = new Version(1, 2, 3),
                    EdgeTX = new Version(3, 4, 5),
                    ExpressLRS = new Version(2, 3, 4),
                    Whoopstor3 = new Version(1, 0, 0)
                };
            }

            if (!Design.IsDesignMode)
            {
                this.TopMostEnabled = Globals.UserConfig.RuntimeConfiguration.TopMost;

                this.OnInstallledSoftwareTimerElapsed(this, null);

                _installedSoftwareTimer = new()
                {
                    Interval = TimeSpan.FromMinutes(5).TotalMilliseconds
                };

                _installedSoftwareTimer.Elapsed += this.OnInstallledSoftwareTimerElapsed;
                _installedSoftwareTimer.Start();

                this.lgsService = new();
                this.lgsService.Start();
                this.lgsService.DataUpdated += this.LgsDataUpdated;

                this.weatherApiService = new WeatherApiService(Globals.UserConfig.RuntimeConfiguration.WeatherApiComApiKey);
                this.weatherApiService.Start();
                this.weatherApiService.DataUpdated += this.WeatherDataUpdated;

                this.fpvService = new();
                this.fpvService.Start();
                this.fpvService.DataUpdated += this.FpvDataUpdated;

                this.softwareService = new();
                this.softwareService.Start();
                this.softwareService.DataUpdated += (s, e) =>
                {
                    this.SoftwareResponse = this.softwareService.Response;
                    if (this.InstalledSoftware != null && this.InstalledSoftware.Godot.Equals(this.SoftwareResponse.Godot))
                    {
                        this.IsGodotLatest = true;
                    }
                };

                Task.Run(async () =>
                {
                    await this.weatherApiService.Run();
                });

                Task.Run(async () =>
                {
                    await this.fpvService.Run();
                });

                Task.Run(async () =>
                {
                    await this.softwareService.Run();
                });
            }
        }
        #endregion

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
            Task.Run(async () => await Globals.UserConfig.Save());
        }

        #region Logitech Devices
        internal readonly LgDeviceService lgsService;
        [ObservableProperty]
        public partial LgsDeviceResponse LgsResponse { get; set; }
        [ObservableProperty]
        public partial LoadingIndicatorMode LgsLoadingIndicatorMode { get; set; }
        private void LgsDataUpdated(object sender, EventArgs e)
        {
            this.LgsResponse = this.lgsService.Response;
            base.OnPropertyChanged(nameof(this.LgsResponse));

            if (this.Instance == null)
            {
                while (this.Instance == null || this.LgsResponse == null || this.LgsResponse.Devices.Count <= 0)
                {
                    Thread.Sleep(100);
                }
            }

            this.Instance.Dispatcher.Invoke(() =>
            {
                StackPanel p = ((MainWindow)this.Instance).LgsStackPanel;

                if (p.Children.Count != this.LgsResponse.Devices.Count)
                {
                    p.Children.Clear();

                    foreach (LogitechDevice d in this.LgsResponse.Devices)
                    {
                        LgsDeviceElement lde = new()
                        {
                            LogitechDevice = d
                        };

                        p.Children.Add(lde);
                    }

                    return;
                }

                foreach (LogitechDevice d in this.LgsResponse.Devices)
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
        #endregion

        #region Weather
        internal readonly WeatherApiService weatherApiService;
        [ObservableProperty]
        public partial WeatherApiResponse WeatherResponse { get; set; }
        private void WeatherDataUpdated(object sender, EventArgs e)
        {
            this.WeatherResponse = this.weatherApiService.Response;
        }
        #endregion

        #region FpvFirmwares
        internal readonly FpvDroneApiService fpvService;
        [ObservableProperty]
        public partial FpvDroneResponse FpvResponse { get; set; }

        private void FpvDataUpdated(object sender, EventArgs e)
        {
            this.FpvResponse = this.fpvService.Response;

            if (this.InstalledSoftware != null)
            {
                this.IsBluejayLatest = this.FpvResponse.BlueJay.Equals(this.InstalledSoftware.Bluejay);
                this.IsBetaflightLatest = this.FpvResponse.BetaflightFw.Equals(this.InstalledSoftware.BetaflightFw);
                this.IsElrsLatest = this.FpvResponse.ExpressLRS.Equals(this.InstalledSoftware.ExpressLrs);
                this.IsEdgeTxLatest = this.FpvResponse.EdgeTX.Equals(this.InstalledSoftware.EdgeTx);
                this.IsWhoopStorLatest = this.FpvResponse.Whoopstor3.Equals(this.InstalledSoftware.Whoopstor);
            }
        }
        #endregion

        #region Software Versions
        internal readonly SoftwareApiService softwareService;
        [ObservableProperty]
        public partial SoftwareResponse SoftwareResponse { get; set; } = new();
        #endregion

        #region Installed Software List
        private readonly System.Timers.Timer _installedSoftwareTimer;
        [ObservableProperty]
        public partial InstalledSoftware InstalledSoftware { get; set; }

        [ObservableProperty]
        public partial bool IsGodotLatest { get; set; }

        [ObservableProperty]
        public partial bool IsBluejayLatest { get; set; }

        [ObservableProperty]
        public partial bool IsBetaflightLatest { get; set; }

        [ObservableProperty]
        public partial bool IsElrsLatest { get; set; }

        [ObservableProperty]
        public partial bool IsEdgeTxLatest { get; set; }

        [ObservableProperty]
        public partial bool IsWhoopStorLatest { get; set; }
        private void OnInstallledSoftwareTimerElapsed(object sender, ElapsedEventArgs e)
        {
            using (FileStream fs = File.Open(Path.Combine(Globals.AppLocalBaseUserPath, "softwarepack.json"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this.InstalledSoftware = JsonSerializer.Deserialize<InstalledSoftware>(fs);
            }
        }
        #endregion
    }
}
