#nullable enable
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

/// <summary>
/// Base class for array mappers which maps provided objects as complex objects and provides access to their properties.
/// </summary>
public abstract class ObjectMapper : NodeMapper
{
    /// <summary>
    /// Constructor
    /// </summary>
    protected ObjectMapper() : base(ObjectTreeNodeKind.Object) { }

    /// <summary>
    /// Interpret provided object <paramref name="o"/> as complex object and returns its properties in form of property name and associated value pairs.
    /// If given property access causes an exception to be thrown, it is captured as <seealso cref="ExceptionCapture"/> instance which is returned instead of the value.
    /// </summary>
    public abstract IEnumerable<ObjectProperty> GetProperties(object o);
}