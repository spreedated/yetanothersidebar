#pragma warning disable S1075

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Docker.DotNet.Models;
using Hardcodet.Wpf.TaskbarNotification;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Serilog;
using Services;
using Services.Models.Responses;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YetAnotherMonitor.Attributes;
using YetAnotherMonitor.Logic;
using YetAnotherMonitor.Views;

namespace YetAnotherMonitor.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        internal readonly RetrieveGasstationPricesService gasstationPricesService;
        internal readonly MicrophoneService microphoneService;
        internal readonly WeatherApiService weatherApiService;
        internal readonly NzbService nzbService;
        internal readonly IssStreamService issService;
        internal readonly HwInfoService hwService;
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
                await this.nzbService.Run();
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
        private MicrophoneResponse microphoneResponse;

        [ObservableProperty]
        private NzbResponse _NzbResponse;

        [ObservableProperty]
        private IssResponse issResponse;

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
        #endregion

        public MainWindowViewModel()
        {
            this.taskbarIcon = new()
            {
                IconSource = new BitmapImage(new Uri(@"pack://application:,,,/resources/proxy_white.ico", UriKind.Absolute))
            };

            this.issService = new();
            this.issService.Start();
            this.issService.DataUpdated += this.IssDataUpdated;

            this.nzbService = new();
            this.nzbService.Start();
            this.nzbService.DataUpdated += this.NzbDataUpdated;

            this.gasstationPricesService = new RetrieveGasstationPricesService();
            this.gasstationPricesService.Start();
            this.gasstationPricesService.DataUpdated += this.GaspricesDataUpdated;
            this.gasstationPricesService.Error += this.GaspricesDataErrorOccurred;

            this.microphoneService = new MicrophoneService();
            this.microphoneService.Start();
            this.microphoneService.DataUpdated += this.MicrophoneDataUpdated;

            this.weatherApiService = new WeatherApiService(RuntimeStorage.Configuration.RuntimeConfiguration.WeatherApiComApiKey);
            this.weatherApiService.Start();
            this.weatherApiService.DataUpdated += this.WeatherDataUpdated;

            this.hwService = new();
            this.hwService.Start();
            this.hwService.DataUpdated += this.HwDataUpdated;

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

            //Task.Run(async () =>
            //{
            //    await this.gasstationPricesService.Run();
            //});

            //Task.Run(async () =>
            //{
            //    await this.microphoneService.Run();
            //});

            //Task.Run(async () =>
            //{
            //    await this.weatherApiService.Run();
            //});

            //Task.Run(async () =>
            //{
            //    await this.nzbService.Run();
            //});

            //Task.Run(async () =>
            //{
            //    await this.issService.Run();
            //});

            Task.Run(async () =>
            {
                await this.hwService.Run();
            });
        }

        [ServiceEvent(typeof(HwInfoService), ServiceEventAttribute.ServiceEventTypes.Updated)]
        private void HwDataUpdated(object sender, EventArgs e)
        {
            this.HwInfoCpuLoad ??= [];
            lock (Sync)
            {
                this.HwInfoCpuLoad.Add((float)Math.Round(this.hwService.Response.Telemetric.FirstOrDefault(x => x.NameDefault == "Total CPU Usage").ValueNow, 2));
                _time++;
                XAxes[0].MinLimit = _time - 60;
                XAxes[0].MaxLimit = _time;
            }
            this.HwInfoCpuPower = (float)Math.Round(this.hwService.Response.Telemetric.FirstOrDefault(x => x.NameDefault == "CPU Package Power").ValueNow, 2);

            this.OnPropertyChanged(nameof(this.HwInfoCpuLoad));
        }

        [ServiceEvent(typeof(IssStreamService), ServiceEventAttribute.ServiceEventTypes.Updated)]
        private void IssDataUpdated(object sender, EventArgs e)
        {
            this.IssResponse = this.issService.Response;
            this.OnPropertyChanged(nameof(this.IssResponse));
        }

        [ServiceEvent(typeof(NzbService), ServiceEventAttribute.ServiceEventTypes.Updated)]
        private void NzbDataUpdated(object sender, EventArgs e)
        {
            this.NzbResponse = this.nzbService.Response;
        }

        [ServiceEvent(typeof(WeatherApiService), ServiceEventAttribute.ServiceEventTypes.Updated)]
        private void WeatherDataUpdated(object sender, EventArgs e)
        {
            this.WeatherResponse = this.weatherApiService.Response;
        }

        [ServiceEvent(typeof(MicrophoneService), ServiceEventAttribute.ServiceEventTypes.Updated)]
        private void MicrophoneDataUpdated(object sender, EventArgs e)
        {
            this.MicrophoneResponse = this.microphoneService.Response;
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
