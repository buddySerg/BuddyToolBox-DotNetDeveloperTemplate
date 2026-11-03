using System.Globalization;
using System.Windows.Data;

namespace BTB.Core.Converters;

/// <summary>
/// Provides a WPF value converter that inverses Boolean values.
/// </summary>
public sealed class InverseBoolConverter : IValueConverter
{
    /// <summary>
    /// Converts a Boolean value to its inverse representation.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Invert(value);
    }

    /// <summary>
    /// Converts a Boolean value back to its inverse representation.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Invert(value);
    }

    /// <summary>
    /// Inverts the specified Boolean value.
    /// Non-Boolean values result in <see cref="Binding.DoNothing"/>.
    /// </summary>
    private static object Invert(object? value)
    {
        return value is bool boolean ? !boolean : Binding.DoNothing;
    }
}
