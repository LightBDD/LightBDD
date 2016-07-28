using LightBDD.Scenarios.Parameterized;

namespace LightBDD
{
    public static class ParameterizedScenarioExtensions
    {
        public static IParameterizedScenarioRunner<TContext> Parameterized<TContext>(this IBddRunner<TContext> runner)
        {
            return new ParameterizedScenarioRunner<TContext>(runner);
        }
    }
}
