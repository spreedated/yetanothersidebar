using MahApps.Metro.IconPacks;
using neXn.Lib.Wpf.ViewLogic;
using System;
using System.Globalization;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal class DegreeToMaterialKindIcon : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!int.TryParse(value.ToString(), out int b))
            {
                return PackIconMaterialKind.DotsHorizontal;
            }

            if (b >= 330 || b <= 30)
            {
                return PackIconMaterialKind.ArrowUpThick;
            }

            if (b >= 60 && b <= 120)
            {
                return PackIconMaterialKind.ArrowRightThick;
            }

            if (b >= 240 && b <= 300)
            {
                return PackIconMaterialKind.ArrowLeftThick;
            }

            if (b >= 150 && b <= 210)
            {
                return PackIconMaterialKind.ArrowDownThick;
            }

            if (b >= 30 && b <= 60)
            {
                return PackIconMaterialKind.ArrowTopRightThick;
            }

            if (b >= 120 && b <= 150)
            {
                return PackIconMaterialKind.ArrowBottomRightThick;
            }

            if (b >= 210 && b <= 240)
            {
                return PackIconMaterialKind.ArrowBottomLeftThick;
            }

            if (b >= 300 && b <= 330)
            {
                return PackIconMaterialKind.ArrowTopLeftThick;
            }

            return PackIconMaterialKind.DotsHorizontal;
        }
    }
}
