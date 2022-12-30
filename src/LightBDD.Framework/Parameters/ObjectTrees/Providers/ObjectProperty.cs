#nullable enable
namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

/// <summary>
/// Structure representing object property with associated value
/// </summary>
public readonly struct ObjectProperty
{
    /// <summary>
    /// Property name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Property value
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="name">Property name</param>
    /// <param name="value">Associated value</param>
    public ObjectProperty(string name, object? value)
    {
        Name = name;
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Name}: {Value}";
}