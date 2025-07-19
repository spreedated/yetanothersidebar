using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;
using System.Windows;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class BoolToVisibilityConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
