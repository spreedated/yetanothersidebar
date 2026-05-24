using neXn.Ui.Avalonia.Converter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YamAva.ViewLogic
{
    public sealed class TitleConverter : MultiConverterBase
    {
        public override object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || !values.All(x => x is string) || values.Count <= 1)
            {
                return null;
            }

            return $"{values[0]}\n{values[1]}";
        }

        public override object ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
