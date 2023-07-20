using System;
using System.Collections.Generic;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Scenario inline case attribute allowing to provide arguments for parameterized scenarios.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ScenarioInlineCaseAttribute : Attribute, IScenarioCaseSourceAttribute
    {
        private readonly object[] _arguments;

        /// <summary>
        /// Constructor allowing to specify scenario arguments.
        /// </summary>
        /// <param name="arguments"></param>
        public ScenarioInlineCaseAttribute(params object[] arguments)
        {
            _arguments = arguments ?? new object[] { null };
        }

        /// <inheritdoc />
        public IEnumerable<object[]> GetCases()
        {
            yield return _arguments;
        }

        /// <summary>
        /// Returns <c>true</c>, informing that these scenario arguments should be resolved at discovery time.
        /// </summary>
        public bool IsResolvableAtDiscovery => true;
    }
}
