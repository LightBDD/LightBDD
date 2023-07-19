using System;
using System.Collections.Generic;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ScenarioInlineCaseAttribute : Attribute, IScenarioCaseSourceAttribute
    {
        private readonly object[] _arguments;

        public ScenarioInlineCaseAttribute(params object[] arguments)
        {
            _arguments = arguments ?? new object[] { null };
        }

        public IEnumerable<object[]> GetCases()
        {
            yield return _arguments;
        }

        public bool IsResolvableAtDiscovery => true;
    }
}
