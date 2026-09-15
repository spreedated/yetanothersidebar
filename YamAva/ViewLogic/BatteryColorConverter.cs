using Avalonia.Media;
using neXn.Ui.Avalonia.Converter;
using System;
using System.Globalization;

namespace YamAva.ViewLogic
{
    public class BatteryColorConverter : ConverterBase
    {
        private static readonly (double Percentage, Color Color)[] BatteryColors =
        [
            (0,   Color.Parse("#F28C8C")),
            (10,  Color.Parse("#F4A3A3")),
            (30,  Color.Parse("#FFDC95")),
            (50,  Color.Parse("#DEFFA1")),
            (75,  Color.Parse("#C8FAA2")),
            (100, Color.Parse("#B2F4A3"))
        ];

        private static IBrush GetBatteryBrush(double percentage)
        {
            percentage = Math.Clamp(percentage, 0d, 100d);

            for (int i = 0; i < BatteryColors.Length - 1; i++)
            {
                var from = BatteryColors[i];
                var to = BatteryColors[i + 1];

                if (percentage > to.Percentage)
                {
                    continue;
                }

                double range = to.Percentage - from.Percentage;
                double t = (percentage - from.Percentage) / range;

                return new SolidColorBrush(
                    Interpolate(from.Color, to.Color, t));
            }

            return new SolidColorBrush(BatteryColors[^1].Color);
        }

        private static Color Interpolate(Color from, Color to, double amount)
        {
            byte Lerp(byte a, byte b) =>
                (byte)Math.Round(a + (b - a) * amount);

            return Color.FromArgb(
                Lerp(from.A, to.A),
                Lerp(from.R, to.R),
                Lerp(from.G, to.G),
                Lerp(from.B, to.B));
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percentage = value switch
            {
                int i => i,
                float f => f,
                double d => d,
                decimal m => (double)m,
                _ => double.NaN
            };

            if (double.IsNaN(percentage))
            {
                return Brushes.Transparent;
            }

            return GetBatteryBrush(percentage);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
