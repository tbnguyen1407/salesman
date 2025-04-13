using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SalesMan.Models;

namespace SalesMan.Converters;

class EntityActionDialogIsEnabledConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EntityAction action)
        {
            return action != EntityAction.View;
        }

        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

class EntityActionDialogVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EntityAction action)
        {
            return action == EntityAction.View ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Visible;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
