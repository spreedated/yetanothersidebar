using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class BoolToTextConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return b ? "Enable" : "Disable";
        }
    }
}
