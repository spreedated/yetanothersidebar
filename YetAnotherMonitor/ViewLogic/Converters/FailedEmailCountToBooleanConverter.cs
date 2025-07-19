using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class FailedEmailCountToBooleanConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int b = (int)value;

            if (b <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
