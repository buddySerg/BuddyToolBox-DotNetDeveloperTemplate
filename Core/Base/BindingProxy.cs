using System.Windows;

namespace BTB.Core.Base;

/// <summary>
/// Provides a bindable proxy for exposing data through a resource dictionary.
/// </summary>
public sealed class BindingProxy : Freezable
{
    /// <summary>
    /// Creates a new instance of the binding proxy.
    /// </summary>
    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }

    /// <summary>
    /// Gets or sets the data exposed by the binding proxy.
    /// </summary>
    public object? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Data"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(
            nameof(Data),
            typeof(object),
            typeof(BindingProxy),
            new PropertyMetadata(null));
}