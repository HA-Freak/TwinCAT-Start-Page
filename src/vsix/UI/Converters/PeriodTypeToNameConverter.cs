// usage:
// xmlns:dat="clr-namespace:System.Windows.Data;assembly=PresentationFramework"
// xmlns:c="clr-namespace:StartPageTcXaeShell64.UI.Converters"

//<dat:PropertyGroupDescription x:Name="GroupByPeriodType" PropertyName="PeriodType" Converter="{c:PeriodTypeToStringConverter}"/>

namespace StartPageTcXaeShell64.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using StartPageTcXaeShell64.UI.Enums;
    using StartPageTcXaeShell64.UI.MarkupExtensions;

    [ValueConversion(typeof(PeriodType), typeof(string))]
    public class PeriodTypeToNameConverter : ValueConverterMarkupExtension
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is PeriodType periodType)
                ? periodType.ToName()
                : value;
    };
}