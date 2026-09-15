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
    public static readonly StyledProperty<LogitechDevice> LogitechDeviceProperty =
    AvaloniaProperty.Register<LgsDeviceElement, LogitechDevice>(nameof(LogitechDevice), defaultBindingMode: BindingMode.TwoWay);

    public LogitechDevice LogitechDevice
    {
        get => base.GetValue(LogitechDeviceProperty);
        set => base.SetValue(LogitechDeviceProperty, value);
    }


    public static readonly StyledProperty<Bitmap> BatteryIndicatorProperty =
        AvaloniaProperty.Register<LgsDeviceElement, Bitmap>(nameof(BatteryIndicator), defaultBindingMode: BindingMode.TwoWay);

    public Bitmap BatteryIndicator
    {
        get => base.GetValue(BatteryIndicatorProperty);
        set => base.SetValue(BatteryIndicatorProperty, value);
    }


    public static readonly StyledProperty<Bitmap> DeviceTypeProperty =
        AvaloniaProperty.Register<LgsDeviceElement, Bitmap>(nameof(DeviceType), defaultBindingMode: BindingMode.TwoWay);

    public Bitmap DeviceType
    {
        get => base.GetValue(DeviceTypeProperty);
        set => base.SetValue(DeviceTypeProperty, value);
    }


    public static readonly StyledProperty<bool> IsChargingProperty =
        AvaloniaProperty.Register<LgsDeviceElement, bool>(nameof(IsCharging), defaultBindingMode: BindingMode.TwoWay);

    public bool IsCharging
    {
        get => base.GetValue(IsChargingProperty);
        set => base.SetValue(IsChargingProperty, value);
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LogitechDeviceProperty && change.NewValue is LogitechDevice device)
        {
            this.SetDeviceType();

            this.IsCharging = device.PowerSupplyStatus == Services.PowerSupplyStatus.POWER_SUPPLY_STATUS_CHARGING;
        }
    }
}