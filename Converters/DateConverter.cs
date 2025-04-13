using System;
using System.Windows.Data;

namespace SalesMan.Converters;

class DateConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd");
        }
        return "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
