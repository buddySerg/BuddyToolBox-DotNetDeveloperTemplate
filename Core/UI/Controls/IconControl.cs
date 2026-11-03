using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Wpf.Ui.Controls;

namespace BTB.Core.UI.Controls;

/// <summary>
/// Provides a WPF control that displays an icon resolved from an application resource.
/// </summary>
public sealed class IconControl : IconElement
{
    #region Public Properties
    /// <summary>
    /// Identifies the <see cref="IconResourceKey"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconResourceKeyProperty =
        DependencyProperty.Register(
            nameof(IconResourceKey),
            typeof(string),
            typeof(IconControl),
            new PropertyMetadata(null, OnIconResourceKeyChanged));

    /// <summary>
    /// Gets or sets the resource key for the icon to be displayed.
    /// </summary>
    public string? IconResourceKey
    {
        get => (string?)GetValue(IconResourceKeyProperty);
        set => SetValue(IconResourceKeyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IconSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(
            nameof(IconSize),
            typeof(double),
            typeof(IconControl),
            new PropertyMetadata(24.0, OnIconSizeChanged));

    /// <summary>
    /// Gets or sets the width and height of the displayed icon.
    /// </summary>
    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    #endregion

    #region Events
    /// <summary>
    /// Updates the displayed icon when the <see cref="IconResourceKey"/> property changes.
    /// </summary>
    private static void OnIconResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((IconControl)d).UpdateIcon();
    }

    /// <summary>
    /// Updates the displayed icon when the <see cref="IconSize"/> property changes.
    /// </summary>
    private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((IconControl)d).UpdateIcon();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes the child element based on the current icon resource and size.
    /// </summary>
    protected override UIElement InitializeChildren()
    {
        if (string.IsNullOrWhiteSpace(IconResourceKey))
        {
            return new Border();
        }

        object? resource = TryFindResource(IconResourceKey) ?? Application.Current?.TryFindResource(IconResourceKey);

        switch (resource)
        {
            case null:
                return new Border();
            case SymbolIconSource symbol:
                return new SymbolIcon
                {
                    Symbol = symbol.Symbol,
                    Width = IconSize,
                    Height = IconSize
                };
            case ImageSource image:
                return new System.Windows.Controls.Image
                {
                    Source = image,
                    Width = IconSize,
                    Height = IconSize,
                    Stretch = Stretch.Uniform
                };
            default:
                return new Border();
        }
    }

    /// <summary>
    /// Invalidates the control layout so that the displayed icon is refreshed.
    /// </summary>
    private void UpdateIcon()
    {
        InvalidateMeasure();
        InvalidateArrange();
    }
    #endregion
}