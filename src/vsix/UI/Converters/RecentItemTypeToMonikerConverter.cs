// usage:
// xmlns:c="clr-namespace:StartPageTcXaeShell64.UI.Converters"

//<Image Moniker="{Binding RecentItemType, Converter={c:RecentItemTypeToImageMonikerConverter}">

namespace StartPageTcXaeShell64.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Microsoft.VisualStudio.Imaging.Interop;

    using StartPageTcXaeShell64.UI.Enums;
    using StartPageTcXaeShell64.UI.MarkupExtensions;

    [ValueConversion(typeof(RecentItemType), typeof(ImageMoniker))]
    public class RecentItemTypeToImageMonikerConverter : ValueConverterMarkupExtension
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is RecentItemType itemType)
                ? itemType.ToImageMoniker()
                : value;
    };
}