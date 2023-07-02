using System.Collections.Generic;

namespace LightBDD.Fixie3
{
    /// <summary>
    /// An interface that should be implemented by attributes providing scenario cases for parameterized scenario methods.
    /// </summary>
    public interface IScenarioCaseSourceAttribute
    {
        /// <summary>
        /// Returns collection of cases for parameterized scenario.
        /// </summary>
        IEnumerable<object[]> GetCases();
    }
}