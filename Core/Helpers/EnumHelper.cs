namespace BTB.Core.Helpers;

public static class EnumHelper
{
    /// <summary>
    /// Gets the enum values paired with their localized labels.
    /// </summary>
    public static IEnumerable<KeyValuePair<TEnum, string>> GetLocalizedItems<TEnum>(Localization.ILocalizationService? localizationService = null, string? prefix = null) where TEnum : struct, Enum
    {
        if (localizationService == null)
        {
            return Enum.GetValues<TEnum>()
                .Select(value => new KeyValuePair<TEnum, string>(value, value.ToString()));
        }
        else
        {
            return Enum.GetValues<TEnum>()
                .Select(value => new KeyValuePair<TEnum, string>(
                    value,
                    localizationService[$"{prefix}.{value}"]));
        }
    }
}