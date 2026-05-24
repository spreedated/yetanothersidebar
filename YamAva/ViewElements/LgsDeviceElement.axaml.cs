using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Services.Models;
using System;

namespace YamAva.ViewElements;

public partial class LgsDeviceElement : UserControl
{
    public static readonly DirectProperty<LgsDeviceElement, LogitechDevice> LogitechDeviceProperty = AvaloniaProperty.RegisterDirect<LgsDeviceElement, LogitechDevice>(nameof(LogitechDevice), o => o.LogitechDevice, (o, v) => o.LogitechDevice = v, defaultBindingMode: BindingMode.TwoWay);
    private LogitechDevice logitechDevice;
    public LogitechDevice LogitechDevice
    {
        get { return this.logitechDevice; }
        set
        {
            LogitechDevice oldValue = value;
            this.logitechDevice = value;
            this.RaisePropertyChanged<LogitechDevice>(LogitechDeviceProperty, oldValue, value);
            this.SetBatteryIndicator();
            this.SetDeviceType();
            this.IsCharging = value.PowerSupplyStatus == Services.PowerSupplyStatus.POWER_SUPPLY_STATUS_CHARGING;
        }
    }

    public static readonly DirectProperty<LgsDeviceElement, Bitmap> BatteryIndicatorProperty = AvaloniaProperty.RegisterDirect<LgsDeviceElement, Bitmap>(nameof(BatteryIndicator), o => o.BatteryIndicator, (o, v) => o.BatteryIndicator = v, defaultBindingMode: BindingMode.TwoWay);
    private Bitmap batteryIndicator;
    public Bitmap BatteryIndicator
    {
        get { return this.batteryIndicator; }
        set
        {
            Bitmap oldValue = value;
            this.batteryIndicator = value;
            this.RaisePropertyChanged<Bitmap>(BatteryIndicatorProperty, oldValue, value);
        }
    }

    public static readonly DirectProperty<LgsDeviceElement, Bitmap> DeviceTypeProperty = AvaloniaProperty.RegisterDirect<LgsDeviceElement, Bitmap>(nameof(DeviceType), o => o.DeviceType, (o, v) => o.DeviceType = v, defaultBindingMode: BindingMode.TwoWay);
    private Bitmap deviceType;
    public Bitmap DeviceType
    {
        get { return this.deviceType; }
        set
        {
            Bitmap oldValue = value;
            this.deviceType = value;
            this.RaisePropertyChanged<Bitmap>(DeviceTypeProperty, oldValue, value);
        }
    }

    public static readonly DirectProperty<LgsDeviceElement, bool> IsChargingProperty = AvaloniaProperty.RegisterDirect<LgsDeviceElement, bool>(nameof(IsCharging), o => o.IsCharging, (o, v) => o.IsCharging = v, defaultBindingMode: BindingMode.TwoWay);
    private bool isCharging;
    public bool IsCharging
    {
        get { return this.isCharging; }
        set
        {
            bool oldValue = value;
            this.isCharging = value;
            this.RaisePropertyChanged<bool>(IsChargingProperty, oldValue, value);
        }
    }

    #region Ctor
    public LgsDeviceElement()
    {
        this.InitializeComponent();

        if (Design.IsDesignMode)
        {
            this.LogitechDevice = new()
            {
                DeviceName = "G915 TKL",
                DeviceType = Services.DeviceType.Keyboard,
                BatteryPercentage = 75d,
                PowerSupplyStatus = Services.PowerSupplyStatus.POWER_SUPPLY_STATUS_CHARGING,
                DeviceId = "123",
                HasBattery = true
            };
        }
    }
    #endregion

    private void SetDeviceType()
    {
        switch (this.LogitechDevice.DeviceType)
        {
            case Services.DeviceType.Keyboard:
                this.DeviceType = new Bitmap(AssetLoader.Open(new Uri("avares://YamAva/Resources/Keyboard_dark.png")));
                break;
            case Services.DeviceType.Mouse:
                this.DeviceType = new Bitmap(AssetLoader.Open(new Uri("avares://YamAva/Resources/Mouse_dark.png")));
                break;
            case Services.DeviceType.Headset:
                this.DeviceType = new Bitmap(AssetLoader.Open(new Uri("avares://YamAva/Resources/Headset_dark.png")));
                break;
            default:
                break;
        }
    }

    private void SetBatteryIndicator()
    {
        if (this.LogitechDevice.BatteryPercentage >= 91)
        {
            this.BatteryIndicator = new Bitmap(AssetLoader.Open(new Uri("avares://YamAva/Resources/Indicator_100.png")));
            return;
        }
        if (this.LogitechDevice.BatteryPercentage >= 50)
        {
            this.BatteryIndicator = new Bitmap(AssetLoader.Open(new Uri("avares://YamAva/Resources/Indicator_50.png")));
            return;
        }
        if (this.LogitechDevice.BatteryPercentage >= 30)
        {
            this.BatteryIndicator = new Bitmap(AssetLoader.Open(new Uri("avares://YamAva/Resources/Indicator_30.png")));
            return;
        }
        if (this.LogitechDevice.BatteryPercentage >= 10)
        {
            this.BatteryIndicator = new Bitmap(AssetLoader.Open(new Uri("avares://YamAva/Resources/Indicator_10.png")));
            return;
        }

        this.BatteryIndicator = new Bitmap(AssetLoader.Open(new Uri("avares://YamAva/Resources/Indicator_10.png")));
    }
}