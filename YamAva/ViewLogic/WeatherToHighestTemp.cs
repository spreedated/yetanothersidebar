using neXn.Ui.Avalonia.Converter;
using Services.Models.Responses;
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
            List<WeatherApiResponse.Forecastday> b = (List<WeatherApiResponse.Forecastday>)value;
            return b.FirstOrDefault()?.Day.MaxtempC;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
