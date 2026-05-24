using Avalonia.Media;
using neXn.Ui.Avalonia.Converter;
using System;
using System.Globalization;

namespace YamAva.ViewLogic
{
    public class BoolToBrushConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return b ? Brushes.Green : Brushes.Red;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
