using System;
using System.Globalization;
using System.Windows.Data;

namespace SQLDataGeneratorApplication
{
    //ref https://stackoverflow.com/a/29076707/4060937
    [ValueConversion(typeof(bool), typeof(bool))]
    public class YesNoBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object noAsEmpty, CultureInfo culture)
        {
            var boolValue = value is bool && (bool)value;

            return boolValue
                ? "Yes"
                : ("true".Equals((string)noAsEmpty, StringComparison.OrdinalIgnoreCase) ? string.Empty : "No"); //null safe
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.ToString() == "Yes";
        }
    }
}