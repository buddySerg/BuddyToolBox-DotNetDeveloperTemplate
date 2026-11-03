using System.Collections;
using System.Reflection;

namespace BTB.Core.Helpers;

/// <summary>
/// Provides methods for recursively comparing object graphs and identifying their differences.
/// </summary>
public static class ObjectComparer
{
    #region Public Methods
    /// <summary>
    /// Compares two object graphs and returns all detected differences, including their paths and values.
    /// </summary>
    public static IReadOnlyList<ObjectDifference> GetDifferences(object? original, object? current)
    {
        List<ObjectDifference> differences = new List<ObjectDifference>();

        CompareObject(original, current, string.Empty, differences);

        return differences;
    }

    /// <summary>
    /// Determines whether two object graphs contain no differences.
    /// </summary>
    public static bool AreEqual(object? original, object? current)
    {
        return GetDifferences(original, current).Count == 0;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Recursively compares two values and records differences in the specified collection.
    /// </summary>
    private static void CompareObject(object? original, object? current, string path, ICollection<ObjectDifference> differences)
    {
        if (ReferenceEquals(original, current))
        {
            return;
        }

        if (original is null || current is null)
        {
            differences.Add(new ObjectDifference(path, original, current));
            return;
        }

        Type type = original.GetType();

        if (type != current.GetType())
        {
            differences.Add(new ObjectDifference(path, original, current));
            return;
        }

        if (IsSimpleType(type))
        {
            if (!Equals(original, current))
            {
                differences.Add(new ObjectDifference(path, original, current));
            }
            return;
        }

        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            CompareCollections((IEnumerable)original, (IEnumerable)current, path, differences);
            return;
        }

        if (original is IDictionary originalDictionary && current is IDictionary currentDictionary)
        {
            CompareDictionaries(originalDictionary, currentDictionary, path, differences);
            return;
        }

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!property.CanRead)
            {
                continue;
            }

            object? originalValue = property.GetValue(original);
            object? currentValue = property.GetValue(current);

            string propertyPath = string.IsNullOrWhiteSpace(path)
                ? property.Name
                : $"{path}.{property.Name}";

            CompareObject(originalValue, currentValue, propertyPath, differences);
        }
    }

    /// <summary>
    /// Compares two enumerable values by their item counts and corresponding item positions.
    /// </summary>
    private static void CompareCollections(IEnumerable original, IEnumerable current, string path, ICollection<ObjectDifference> differences)
    {
        object[] originalItems = [.. original.Cast<object>()];
        object[] currentItems = [.. current.Cast<object>()];

        if (originalItems.Length != currentItems.Length)
        {
            differences.Add(new ObjectDifference($"{path}.Count", originalItems.Length, currentItems.Length));
        }

        int count = Math.Min(originalItems.Length, currentItems.Length);

        for (int i = 0; i < count; i++)
        {
            CompareObject(originalItems[i], currentItems[i], $"{path}[{i}]", differences);
        }
    }

    /// <summary>
    /// Compares the entries of two dictionaries and records missing, additional, or changed values.
    /// </summary>
    private static void CompareDictionaries(IDictionary original, IDictionary current, string path, ICollection<ObjectDifference> differences)
    {
        foreach (object key in original.Keys)
        {
            string itemPath = string.IsNullOrWhiteSpace(path)
                ? $"[{key}]"
                : $"{path}[{key}]";

            if (!current.Contains(key))
            {
                differences.Add(new ObjectDifference(itemPath, original[key], null));
                continue;
            }

            CompareObject(original[key], current[key], itemPath, differences);
        }

        foreach (object key in current.Keys)
        {
            if (original.Contains(key))
            {
                continue;
            }

            string itemPath = string.IsNullOrWhiteSpace(path)
                ? $"[{key}]"
                : $"{path}[{key}]";

            differences.Add(new ObjectDifference(itemPath, null, current[key]));
        }
    }

    /// <summary>
    /// Determines whether a type can be compared as a scalar value instead of being traversed as an object graph.
    /// </summary>
    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(Guid)
            || type == typeof(TimeSpan);
    }
    #endregion
}