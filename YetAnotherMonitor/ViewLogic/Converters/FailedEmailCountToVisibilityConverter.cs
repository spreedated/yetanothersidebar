using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;
using System.Windows;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class FailedEmailCountToVisibilityConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int b = (int)value;

            if (b <= 0)
            {
                return Visibility.Hidden;
            }

            return Visibility.Visible;
        }
    }
}
