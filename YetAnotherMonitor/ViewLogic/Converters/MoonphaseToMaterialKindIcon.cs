using MahApps.Metro.IconPacks;
using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class MoonphaseToMaterialKindIcon : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string b = (string)value;

            switch (b.ToLower())
            {
                case "new moon":
                    return PackIconMaterialKind.MoonNew;
                case "waxing crescent":
                    return PackIconMaterialKind.MoonWaxingCrescent;
                case "first quarter":
                    return PackIconMaterialKind.MoonFirstQuarter;
                case "waxing gibbous":
                    return PackIconMaterialKind.MoonWaxingGibbous;
                case "full moon":
                    return PackIconMaterialKind.MoonFull;
                case "waning gibbous":
                    return PackIconMaterialKind.MoonWaningGibbous;
                case "last quarter":
                    return PackIconMaterialKind.MoonLastQuarter;
                case "waning crescent":
                    return PackIconMaterialKind.MoonWaningCrescent;
            }

            return PackIconMaterialKind.DotsHorizontal;
        }
    }
}
