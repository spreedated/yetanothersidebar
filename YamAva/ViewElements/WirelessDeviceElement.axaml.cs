using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace YamAva.ViewElements;

public partial class WirelessDeviceElement : UserControl
{
    public static readonly StyledProperty<string> DeviceNameProperty =
    AvaloniaProperty.Register<WirelessDeviceElement, string>(nameof(DeviceName), defaultBindingMode: BindingMode.TwoWay);

    public string DeviceName
    {
        get => base.GetValue(DeviceNameProperty);
        set => base.SetValue(DeviceNameProperty, value);
    }


    public static readonly StyledProperty<Bitmap> BatteryIndicatorProperty =
        AvaloniaProperty.Register<WirelessDeviceElement, Bitmap>(nameof(BatteryIndicator), defaultBindingMode: BindingMode.TwoWay);

    public Bitmap BatteryIndicator
    {
        get => base.GetValue(BatteryIndicatorProperty);
        set => base.SetValue(BatteryIndicatorProperty, value);
    }


    public static readonly StyledProperty<IImage> DeviceIconProperty =
        AvaloniaProperty.Register<WirelessDeviceElement, IImage>(nameof(DeviceIcon), defaultBindingMode: BindingMode.TwoWay);

    public IImage DeviceIcon
    {
        get => base.GetValue(DeviceIconProperty);
        set => base.SetValue(DeviceIconProperty, value);
    }


    public static readonly StyledProperty<bool> IsChargingProperty =
        AvaloniaProperty.Register<WirelessDeviceElement, bool>(nameof(IsCharging), defaultBindingMode: BindingMode.TwoWay);

    public bool IsCharging
    {
        get => base.GetValue(IsChargingProperty);
        set => base.SetValue(IsChargingProperty, value);
    }


    public static readonly StyledProperty<int> BatteryPercentageProperty =
        AvaloniaProperty.Register<WirelessDeviceElement, int>(nameof(BatteryPercentage), defaultBindingMode: BindingMode.TwoWay);

    public int BatteryPercentage
    {
        get => base.GetValue(BatteryPercentageProperty);
        set => base.SetValue(BatteryPercentageProperty, value);
    }

    public static readonly StyledProperty<bool> IsDisconnectedProperty = AvaloniaProperty.Register<WirelessDeviceElement, bool>(nameof(IsDisconnected));

    public bool IsDisconnected
    {
        get => base.GetValue(IsDisconnectedProperty);
        set => base.SetValue(IsDisconnectedProperty, value);
    }

    #region Ctor
    public WirelessDeviceElement()
    {
        this.InitializeComponent();

        if (Design.IsDesignMode)
        {
            this.DeviceName = "WRLS";
            this.BatteryPercentage = 100;
            this.IsCharging = false;
            this.IsDisconnected = false;
        }
    }
    #endregion
}