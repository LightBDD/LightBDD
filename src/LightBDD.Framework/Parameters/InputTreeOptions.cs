#nullable enable
using LightBDD.Framework.Parameters.ObjectTrees;

namespace LightBDD.Framework.Parameters;

/// <summary>
/// Options for <seealso cref="InputTree{TData}"/>
/// </summary>
public class InputTreeOptions
{
    /// <summary>
    /// Determines if <seealso cref="ObjectTreeObject"/> properties of null values should be excluded or captured on input tree. By default, they are captured.
    /// </summary>
    public bool ExcludeNullProperties { get; set; } = false;
}