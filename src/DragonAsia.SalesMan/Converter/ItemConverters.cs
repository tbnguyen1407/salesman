using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonAsia.SalesMan.Converter
{
    class ItemDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = (string)value;

            return string.IsNullOrWhiteSpace(input) ? "" : "(" + input.Trim() + ")";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
