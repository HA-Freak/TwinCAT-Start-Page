namespace TwinCAT.XAE.StartPage.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using MarkupExtensions;

    //usage:
    //<ColumnDefinition Width="{Binding IsMinimized, Converter={c:BoolToGridLengthConverter Reverse=True}}"/>

    // true: a star column that shares the width with the others, false: as wide as its content
    [ValueConversion(typeof(bool), typeof(GridLength))]
    public class BoolToGridLengthConverter : ValueConverterMarkupExtension
    {
        public bool Reverse { get; set; } = false;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return GridLength.Auto;

            if (Reverse)
            {
                boolValue = !boolValue;
            }

            return boolValue
                ? new GridLength(1, GridUnitType.Star)
                : GridLength.Auto;
        }
    }
}
