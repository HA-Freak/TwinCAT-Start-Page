// usage:
// xmlns:c="clr-namespace:TwinCAT.XAE.StartPage.UI.Converters"

//<Image Moniker="{Binding RecentItemType, Converter={c:RecentItemTypeToImageMonikerConverter}">

namespace TwinCAT.XAE.StartPage.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Microsoft.VisualStudio.Imaging.Interop;

    using TwinCAT.XAE.StartPage.UI.Enums;
    using TwinCAT.XAE.StartPage.UI.MarkupExtensions;

    [ValueConversion(typeof(RecentItemType), typeof(ImageMoniker))]
    public class RecentItemTypeToImageMonikerConverter : ValueConverterMarkupExtension
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is RecentItemType itemType)
                ? itemType.ToImageMoniker()
                : value;
    };
}