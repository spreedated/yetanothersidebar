using neXn.Ui.Avalonia.Converter;
using System;
using System.Globalization;

namespace YamAva.ViewLogic
{
    internal class DecideBytesPerSecondConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float bps = (float)value;

            if (bps >= 1_000_000)
            {
                return $"{(bps * 8) / 1_000_000f:0.##} Mbps";
            }
            else if (bps >= 1000)
            {
                return $"{(bps * 8) / 1000f:0.##} Kbps";
            }
            else
            {
                return $"{(bps * 8):0.##} bps";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
