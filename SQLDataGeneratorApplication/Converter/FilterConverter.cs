using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SQLDataGeneratorApplication
{
    public sealed class FilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (IEnumerable<object>)values[0];
            var field = (string)values[1];
            var value = values[2];

            //TODO take into Util
            var property = list.GetType().GenericTypeArguments[0].GetProperty(field);
            Func<object, bool> predicate = (x) => (dynamic)property.GetValue(x) == (dynamic)value;
            return list.Where(predicate).ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}