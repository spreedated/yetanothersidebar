using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class FloatBelow25to10 : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float f = (float)value;
            return f <= 25f && f > 10f;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float f = (float)value;
            return f <= 25f && f > 10f;
        }
    }
}
