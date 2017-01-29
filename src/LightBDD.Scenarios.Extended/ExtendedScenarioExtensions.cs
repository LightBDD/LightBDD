using LightBDD.Scenarios.Extended.Implementation;

namespace LightBDD.Scenarios.Extended
{
    public static class ExtendedScenarioExtensions
    {
        public static IExtendedScenarioRunner<TContext> Parameterized<TContext>(this IBddRunner<TContext> runner)
        {
            return new ExtendedScenarioRunner<TContext>(runner);
        }
    }
}
