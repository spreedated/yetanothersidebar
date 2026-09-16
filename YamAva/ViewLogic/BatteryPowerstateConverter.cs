using neXn.Ui.Avalonia.Converter;
using System;
using System.Globalization;

namespace YamAva.ViewLogic
{
    public class BatteryPowerstateConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SteamControllerPowerState scp)
            {
                return null;
            }

            return scp == SteamControllerPowerState.Charging;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
