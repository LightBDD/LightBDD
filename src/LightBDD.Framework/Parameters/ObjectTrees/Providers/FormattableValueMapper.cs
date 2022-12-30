#nullable enable
using System;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class FormattableValueMapper : ValueMapper
{
    public static readonly FormattableValueMapper Instance = new();
    private FormattableValueMapper() { }

    public override bool CanMap(object obj) => obj is IFormattable or ISelfFormattable;
    public override object GetValue(object o) => o;
}