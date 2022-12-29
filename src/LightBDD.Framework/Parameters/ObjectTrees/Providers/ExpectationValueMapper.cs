#nullable enable
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class ExpectationValueMapper : ValueMapper
{
    public static readonly ExpectationValueMapper Instance = new();
    private ExpectationValueMapper() { }

    public override bool CanMap(object obj) => obj is IGeneralExpectationConverter;
    public override object GetValue(object o) => ((IGeneralExpectationConverter)o).ToGeneralExpectation();
}