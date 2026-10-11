using neXn.Ui.Avalonia.Converter;
using System;
using System.Globalization;

namespace YamAva.ViewLogic
{
    internal class DecideBytesPerSecondConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float bps = (float)value * 8;

            if (bps >= 1_000_000_000)
            {
                return $"{bps / 1_000_000_000f:0.##} Gbps";
            }

            if (bps >= 1_000_000)
            {
                return $"{bps / 1_000_000f:0.##} Mbps";
            }

            if (bps >= 1_000)
            {
                return $"{bps / 1_000f:0.##} Kbps";
            }

            return $"{bps:0.##} bps";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
