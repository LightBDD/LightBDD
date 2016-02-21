using LightBDD.Core.Extensibility;
using LightBDD.Scenarios.Basic;

namespace LightBDD
{
    public static class BasicScenarioExtensions
    {
        public static IBasicScenarioRunner Basic(this IBddRunner runner)
        {
            return new BasicScenarioRunner(runner.Integrate());
        }
    }
}
