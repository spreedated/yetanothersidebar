using neXn.Ui.Avalonia.Converter;
using System;
using System.Globalization;

namespace YamAva.ViewLogic
{
    public class BatteryHeightConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percentage = value switch
            {
                int i => i,
                float f => f,
                double d => d,
                decimal m => (double)m,
                _ => 0d
            };

            if (!double.TryParse(parameter?.ToString(), CultureInfo.InvariantCulture, out double maxHeight))
            {
                maxHeight = 36d;
            }

            percentage = Math.Clamp(percentage, 0d, 100d);

            return maxHeight * percentage / 100d;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
