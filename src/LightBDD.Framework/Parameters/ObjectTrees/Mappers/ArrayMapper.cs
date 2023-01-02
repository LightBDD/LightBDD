#nullable enable
namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Base class for array mappers which maps provided objects as collections and provides access to their items.
/// </summary>
public abstract class ArrayMapper : NodeMapper
{
    /// <summary>
    /// Constructor
    /// </summary>
    protected ArrayMapper() : base(ObjectTreeNodeKind.Array) { }

    /// <summary>
    /// Interprets the <paramref name="o"/> as array and returns all it's items.
    /// </summary>
    public abstract ArrayMap MapArray(object o);
}