using System;
using System.Globalization;
using System.Windows.Data;

namespace SalesMan.Converters;

public class ShortIdConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var input = (string?)value;
        return input == null ? new string(' ', 10) : input[..10];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
