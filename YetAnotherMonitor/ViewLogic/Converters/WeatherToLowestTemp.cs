using neXn.Lib.Wpf.ViewLogic;
using Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class WeatherToLowestTemp : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<WeatherApiResponse.Forecastday> b = (List<WeatherApiResponse.Forecastday>)value;
            return b.FirstOrDefault()?.Day.MintempC;
        }
    }
}
