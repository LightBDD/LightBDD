using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fixie;

namespace LightBDD.Fixie2.Implementation
{
    internal class ScenarioCaseProvider : ParameterSource
    {
        public IEnumerable<object[]> GetParameters(MethodInfo method)
        {
            return method.GetCustomAttributes().OfType<IScenarioCaseSourceAttribute>().SelectMany(s => s.GetCases());
        }
    }
}