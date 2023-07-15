using System.Collections.Generic;

namespace LightBDD.Core.Extensibility;

/// <summary>
/// An interface that should be implemented by attributes providing scenario cases for parameterized scenario methods.
/// </summary>
public interface IScenarioCaseSourceAttribute
{
    /// <summary>
    /// Returns collection of cases for parameterized scenario.
    /// </summary>
    IEnumerable<object[]> GetCases();

    /// <summary>
    /// When <c>true</c>, the test cases are resolved at discovery, otherwise the test cases are resolved at execution stage.<br/>
    /// It should only return <c>true</c> for simple types and when cases generation is stable does not require complex logic.<br/>
    /// </summary>
    bool IsResolvableAtDiscovery { get; }
}