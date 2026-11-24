using System.Globalization;
using System.Windows;
using System.Windows.Data;

using BTB.Core.Localization;

using Microsoft.Extensions.DependencyInjection;

namespace BTB.Core.Converters;

public sealed class LocalizedEnumConverter : IValueConverter
{
    private readonly ILocalizationService _translate = ((App)Application.Current).Services.GetRequiredService<ILocalizationService>();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Enum enumValue)
        {
            return string.Empty;
        }

        string prefix = parameter?.ToString() ?? throw new ArgumentNullException(nameof(parameter));

        string key = $"{prefix}.{enumValue}";

        return _translate[key];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
