using neXn.Ui.Avalonia.Converter;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YamAva.ViewLogic
{
    public class WeatherToHighestTemp : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<Forecastday> b = (List<Forecastday>)value;
            return b.FirstOrDefault()?.Day.MaxtempC;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
