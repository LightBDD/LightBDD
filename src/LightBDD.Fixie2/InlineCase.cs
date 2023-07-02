using System;
using System.Collections.Generic;

namespace LightBDD.Fixie3
{
    /// <summary>
    /// Attribute allowing to declare parameters for parameterized scenarios.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InlineCase : Attribute, IScenarioCaseSourceAttribute
    {
        private readonly object[] _arguments;

        /// <summary>
        /// Constructor accepting array of parameters that should correspond to the number and types of scenario method parameters.
        /// </summary>
        /// <param name="arguments"></param>
        public InlineCase(params object[] arguments)
        {
            _arguments = arguments;
        }

        IEnumerable<object[]> IScenarioCaseSourceAttribute.GetCases()
        {
            return new[] { _arguments };
        }
    }
}