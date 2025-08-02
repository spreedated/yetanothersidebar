#pragma warning disable S1075

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hardcodet.Wpf.TaskbarNotification;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using Services;
using Services.Models;
using Services.Models.Responses;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YetAnotherMonitor.Attributes;
using YetAnotherMonitor.Logic;
using YetAnotherMonitor.ViewEelements;
using YetAnotherMonitor.Views;

namespace YetAnotherMonitor.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        internal readonly RetrieveGasstationPricesService gasstationPricesService;
        internal readonly WeatherApiService weatherApiService;
        internal readonly LgDeviceService lgsService;
        internal TaskbarIcon taskbarIcon;

        [RelayCommand]
        private void OpenConversions()
        {
            Window convWindow = new Conversions
            {
                Owner = this.Instance
            };
            convWindow.ShowDialog();
        }

        [RelayCommand]
        private void Refresh()
        {
            Task.Run(async () =>
            {
                await this.gasstationPricesService.Run();
                await this.weatherApiService.Run();
            });
        }

        [RelayCommand]
        private void Exit()
        {
            this.Instance.Close();
        }

        #region Bindable Properties
        [ObservableProperty]
        private Window instance;

        public static string WindowTitle
        {
            get
            {
                return $"{typeof(MainWindowViewModel).Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title}";
            }
        }

        [ObservableProperty]
        private Visibility itemVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility loadingItemsVisibility;

        [ObservableProperty]
        private bool itemsEnabled = true;

        [ObservableProperty]
        private ContextMenu contextMenuTaskbar;

        partial void OnContextMenuTaskbarChanged(ContextMenu value)
        {
            this.taskbarIcon.ContextMenu = value;
        }

        [ObservableProperty]
        private Visibility errorVisible = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility noConnectionVisible = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility noConnectionVisiblePR = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility itemVisibilityPR = Visibility.Visible;

        [ObservableProperty]
        private GasstationPrices gaspriceData = new();

        [ObservableProperty]
        private Visibility noConnectionVisiblePostgres = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility itemVisibilityPostgres = Visibility.Visible;

        [ObservableProperty]
        private WeatherApiResponse weatherResponse;

        [ObservableProperty]
        private BindingList<float> hwInfoCpuLoad;

        [ObservableProperty]
        private float hwInfoCpuPower;

        [ObservableProperty]
        private ObservableCollection<ISeries> cpuLoadSeries;

        [ObservableProperty]
        private Axis[] xAxes;

        [ObservableProperty]
        private Axis[] yAxes;

        public object Sync { get; } = new object();
        private int _time = 0;

        [ObservableProperty]
        private LgsDeviceResponse lgsResponse;
        #endregion

        public MainWindowViewModel()
        {
            this.taskbarIcon = new()
            {
                IconSource = new BitmapImage(new Uri(@"pack://application:,,,/resources/icon.ico", UriKind.Absolute))
            };

            this.gasstationPricesService = new RetrieveGasstationPricesService();
            this.gasstationPricesService.Start();
            this.gasstationPricesService.DataUpdated += this.GaspricesDataUpdated;
            this.gasstationPricesService.Error += this.GaspricesDataErrorOccurred;

            this.weatherApiService = new WeatherApiService(RuntimeStorage.Configuration.RuntimeConfiguration.WeatherApiComApiKey);
            this.weatherApiService.Start();
            this.weatherApiService.DataUpdated += this.WeatherDataUpdated;

            this.lgsService = new LgDeviceService();
            this.lgsService.Start();
            this.lgsService.DataUpdated += this.LgsDataUpdated;

            this.CpuLoadSeries = [ new LineSeries<float>()
                                {
                                    Values = this.HwInfoCpuLoad,
                                    Fill = null,
                                    GeometryFill = null,
                                    GeometryStroke = null
                                }];

            XAxes =
            [
                new()
                {
                    IsVisible = false
                },
            ];

            YAxes =
            [
                new()
                {
                    MinLimit = 0,
                    MaxLimit = 100,
                    Name = "CPU Load (%)"
                }
            ];

            Task.Run(async () =>
            {
                await this.gasstationPricesService.Run();
            });


            Task.Run(async () =>
            {
                await this.weatherApiService.Run();
            });

            Task.Run(async () =>
            {
                await this.lgsService.Run();
            });
        }

        [ServiceEvent(typeof(WeatherApiService), ServiceEventAttribute.ServiceEventTypes.Updated)]
        private void WeatherDataUpdated(object sender, EventArgs e)
        {
            this.WeatherResponse = this.weatherApiService.Response;
        }

        [ServiceEvent(typeof(RetrieveGasstationPricesService), ServiceEventAttribute.ServiceEventTypes.Error)]
        private void GaspricesDataErrorOccurred(object sender, Exception e)
        {
            Log.Error($"Gasprices data connection error");

            this.ItemVisibilityPR = Visibility.Collapsed;
            this.NoConnectionVisiblePR = Visibility.Visible;
        }

        [ServiceEvent(typeof(RetrieveGasstationPricesService), ServiceEventAttribute.ServiceEventTypes.Updated)]
        private void GaspricesDataUpdated(object sender, EventArgs e)
        {
            this.GaspriceData = this.gasstationPricesService.Response;
            base.OnPropertyChanged(nameof(this.GaspriceData));
            this.ItemVisibilityPR = Visibility.Visible;
            this.NoConnectionVisiblePR = Visibility.Collapsed;
        }

        [ServiceEvent(typeof(LgDeviceService), ServiceEventAttribute.ServiceEventTypes.Updated)]
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

        public void DisposeNotifyIcon()
        {
            this.taskbarIcon?.Dispose();
        }

        #region Loading Animation
        private CancellationTokenSource animationCts;
        private bool isAnimationRunning;
        public void StartLoadingAnimation()
        {
            if (this.isAnimationRunning)
            {
                return;
            }
            this.isAnimationRunning = true;
            this.animationCts = new();

            Task.Run(this.Animation);
        }

        public void StopLoadingAnimation()
        {
            this.animationCts?.Cancel();
            this.animationCts?.Dispose();
        }

        private async Task Animation()
        {
            this.LoadingItemsVisibility = Visibility.Visible;
            this.ItemVisibility = Visibility.Collapsed;
            this.NoConnectionVisible = Visibility.Collapsed;
            this.ErrorVisible = Visibility.Collapsed;

            this.ItemsEnabled = false;
            while (!this.animationCts.IsCancellationRequested)
            {
                if (this.LoadingItemsVisibility == Visibility.Hidden)
                {
                    this.LoadingItemsVisibility = Visibility.Visible;
                }
                else
                {
                    this.LoadingItemsVisibility = Visibility.Hidden;
                }
                await Task.Delay(800);
            }
            this.ItemVisibility = Visibility.Visible;
            this.LoadingItemsVisibility = Visibility.Collapsed;
            this.ItemsEnabled = true;
            this.isAnimationRunning = false;
        }
        #endregion

        #region Error Animation
        private CancellationTokenSource errorAnimationCts;
        public void StartErrorAnimation()
        {
            this.errorAnimationCts = new();
            Task.Run(this.ErrorAnimation);
        }

        public void StopErrorAnimation()
        {
            this.errorAnimationCts?.Cancel();
            this.errorAnimationCts?.Dispose();
        }

        private async Task ErrorAnimation()
        {
            this.LoadingItemsVisibility = Visibility.Visible;
            this.ItemVisibility = Visibility.Collapsed;
            this.NoConnectionVisible = Visibility.Collapsed;

            this.ItemsEnabled = false;
            while (!this.errorAnimationCts.IsCancellationRequested)
            {
                this.ItemVisibility = Visibility.Collapsed;
                this.LoadingItemsVisibility = Visibility.Collapsed;
                if (this.ErrorVisible == Visibility.Collapsed)
                {
                    this.ErrorVisible = Visibility.Visible;
                }
                else
                {
                    this.ErrorVisible = Visibility.Collapsed;
                }
                await Task.Delay(500);
            }
            this.LoadingItemsVisibility = Visibility.Collapsed;
            this.ItemVisibility = Visibility.Visible;
            this.ErrorVisible = Visibility.Collapsed;
            this.ItemsEnabled = true;
        }
        #endregion
    }
}
