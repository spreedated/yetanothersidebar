using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;
using System.Windows;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class VisibilityReverseConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility b = (Visibility)value;
            return b switch
            {
                Visibility.Visible => Visibility.Collapsed,
                Visibility.Hidden => Visibility.Visible,
                Visibility.Collapsed => Visibility.Visible,
                _ => (object)Visibility.Visible,
            };
        }
    }
}
