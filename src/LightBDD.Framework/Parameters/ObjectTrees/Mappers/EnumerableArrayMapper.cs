using System.Collections;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Mapper for collections implementing <seealso cref="IEnumerable"/> interface.
/// </summary>
public class EnumerableArrayMapper : ArrayMapper
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly EnumerableArrayMapper Instance = new();
    private EnumerableArrayMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> is <seealso cref="IEnumerable"/>
    /// </summary>
    public override bool CanMap(object obj, ObjectTreeBuilderOptions options) => obj is IEnumerable;

    /// <summary>
    /// Maps provided instance to array and returns all it's items in enumeration order of the underlying collection.
    /// </summary>
    public override ArrayMap MapArray(object o, ObjectTreeBuilderOptions options)
    {
        return new ArrayMap(((IEnumerable)o).Cast<object>());
    }
}