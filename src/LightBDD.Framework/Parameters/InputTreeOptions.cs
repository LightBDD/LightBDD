#nullable enable
using LightBDD.Framework.Parameters.ObjectTrees;

namespace LightBDD.Framework.Parameters;

/// <summary>
/// Options for <seealso cref="InputTree{TData}"/>. This class implements immutable pattern, thus any alterations to the options creates a new instance.
/// </summary>
public class InputTreeOptions
{
    /// <summary>
    /// Default options with <seealso cref="ExcludeNullProperties"/> set to <c>false</c>.
    /// </summary>
    public static readonly InputTreeOptions Default = new();

    /// <summary>
    /// Determines if <seealso cref="ObjectTreeObject"/> properties of null values should be excluded or captured on input tree. By default, they are captured.
    /// </summary>
    public bool ExcludeNullProperties { get; private set; } = false;

    /// <summary>
    /// Updates <seealso cref="ExcludeNullProperties"/> with <paramref name="enabled"/> value.
    /// </summary>
    public InputTreeOptions WithExcludeNullProperties(bool enabled)
    {
        var clone = Clone();
        clone.ExcludeNullProperties = enabled;
        return clone;
    }

    private InputTreeOptions() { }
    private InputTreeOptions Clone() => (InputTreeOptions)MemberwiseClone();
}