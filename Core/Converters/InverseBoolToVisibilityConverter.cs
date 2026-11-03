using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BTB.Core.Converters;

/// <summary>
/// Converts a Boolean value to a WPF visibility value using inverse logic.
/// </summary>
public sealed class InverseBoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts <see langword="false"/> to <see cref="Visibility.Visible"/> and all other values to <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolean && !boolean
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <summary>
    /// Converts <see cref="Visibility.Visible"/> to <see langword="false"/> and other visibility values to <see langword="true"/>.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is Visibility visibility
            ? visibility != Visibility.Visible
            : Binding.DoNothing;
    }
}