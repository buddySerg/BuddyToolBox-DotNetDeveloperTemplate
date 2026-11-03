namespace BTB.Core.Helpers;

/// <summary>
/// Represents a difference between two objects.
/// </summary>
public sealed record ObjectDifference(
    string PropertyPath,
    object? OriginalValue,
    object? CurrentValue);
