using CommunityToolkit.Mvvm.ComponentModel;
using Services.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YetAnotherMonitor.ViewEelements
{
    [ObservableObject]
    public partial class LgsDeviceElement : UserControl
    {
        public static readonly DependencyProperty LogitechDeviceProperty = DependencyProperty.Register("LogitechDevice", typeof(LogitechDevice), typeof(LgsDeviceElement));

        public LogitechDevice LogitechDevice
        {
            get => (LogitechDevice)base.GetValue(LogitechDeviceProperty);
            set
            {
                base.SetValue(LogitechDeviceProperty, value);
                this.SetBatteryIndicator();
                this.SetDeviceType();
                this.IsCharging = value.PowerSupplyStatus == Services.PowerSupplyStatus.POWER_SUPPLY_STATUS_CHARGING ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        [ObservableProperty]
        private ImageSource batteryIndicator;

        [ObservableProperty]
        private ImageSource deviceType;

        [ObservableProperty]
        private Visibility isCharging = Visibility.Collapsed;

        #region Ctor
        public LgsDeviceElement()
        {
            this.InitializeComponent();
        }
        #endregion

        private void SetDeviceType()
        {
            switch (this.LogitechDevice.DeviceType)
            {
                case Services.DeviceType.Keyboard:
                    this.DeviceType = new BitmapImage(new Uri("pack://application:,,,/Resources/Keyboard_dark.png"));
                    break;
                case Services.DeviceType.Mouse:
                    this.DeviceType = new BitmapImage(new Uri("pack://application:,,,/Resources/Mouse_dark.png"));
                    break;
                case Services.DeviceType.Headset:
                    this.DeviceType = new BitmapImage(new Uri("pack://application:,,,/Resources/Headset_dark.png"));
                    break;
                default:
                    break;
            }
        }

        private void SetBatteryIndicator()
        {
            if (this.LogitechDevice.BatteryPercentage >= 91)
            {
                this.BatteryIndicator = new BitmapImage(new Uri("pack://application:,,,/Resources/Indicator_100.png"));
                return;
            }
            if (this.LogitechDevice.BatteryPercentage >= 50)
            {
                this.BatteryIndicator = new BitmapImage(new Uri("pack://application:,,,/Resources/Indicator_50.png"));
                return;
            }
            if (this.LogitechDevice.BatteryPercentage >= 30)
            {
                this.BatteryIndicator = new BitmapImage(new Uri("pack://application:,,,/Resources/Indicator_30.png"));
                return;
            }
            if (this.LogitechDevice.BatteryPercentage >= 10)
            {
                this.BatteryIndicator = new BitmapImage(new Uri("pack://application:,,,/Resources/Indicator_10.png"));
                return;
            }

            this.BatteryIndicator = new BitmapImage(new Uri("pack://application:,,,/Resources/Indicator_10.png"));
        }
    }
}
