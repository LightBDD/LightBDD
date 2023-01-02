#nullable enable
namespace LightBDD.Framework.Expectations;

/// <summary>
/// Interface allowing object conversion to general expectation.
/// </summary>
public interface IGeneralExpectationConverter
{
    /// <summary>
    /// Returns the general expectation constructed from current object.
    /// </summary>
    /// <returns></returns>
    Expectation<object?> ToGeneralExpectation();
}