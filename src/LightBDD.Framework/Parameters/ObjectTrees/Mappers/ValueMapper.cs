#nullable enable
namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Base class for array mappers which maps provided objects as simple value.
/// </summary>
public abstract class ValueMapper : NodeMapper
{
    /// <summary>
    /// Constructor
    /// </summary>
    protected ValueMapper() : base(ObjectTreeNodeKind.Value) { }

    /// <summary>
    /// Interprets provided object <paramref name="o"/> as simple value and returns it.
    /// </summary>
    public abstract object? MapValue(object o, ObjectTreeBuilderOptions options);
}