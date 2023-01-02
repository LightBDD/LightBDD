#nullable enable
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Mapper for types implementing <seealso cref="IGeneralExpectationConverter"/>.
/// </summary>
public class ExpectationValueMapper : ValueMapper
{
    /// <summary>
    /// Default instance.
    /// </summary>
    public static readonly ExpectationValueMapper Instance = new();
    private ExpectationValueMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> implements <seealso cref="IGeneralExpectationConverter"/>.
    /// </summary>
    public override bool CanMap(object obj) => obj is IGeneralExpectationConverter;

    /// <summary>
    /// Returns object converted to general expectation representation.
    /// </summary>
    public override object MapValue(object o) => ((IGeneralExpectationConverter)o).ToGeneralExpectation();
}