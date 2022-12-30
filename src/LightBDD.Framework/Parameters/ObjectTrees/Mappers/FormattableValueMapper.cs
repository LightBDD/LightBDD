#nullable enable
using System;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Value mapper for types implementing <seealso cref="IFormattable"/> or <seealso cref="ISelfFormattable"/> interfaces.
/// </summary>
public class FormattableValueMapper : ValueMapper
{
    /// <summary>
    /// Default instance.
    /// </summary>
    public static readonly FormattableValueMapper Instance = new();
    private FormattableValueMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> implements <seealso cref="IFormattable"/> or <seealso cref="ISelfFormattable"/> interface.
    /// </summary>
    public override bool CanMap(object obj) => obj is IFormattable or ISelfFormattable;
    /// <summary>
    /// Returns <paramref name="o"/> as is.
    /// </summary>
    public override object GetValue(object o) => o;
}