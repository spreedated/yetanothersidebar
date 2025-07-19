using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class TimeStringToTimeSpan : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string b = (string)value;
            return DateTime.TryParse(b, out DateTime d) ? d : default;
        }
    }
}
