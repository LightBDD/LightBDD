namespace LightBDD.Core.Extensibility
{
    public static class BddRunnerExtensions
    {
        public static ICoreBddRunner Integrate<T>(this IBddRunner<T> runner)
        {
            return (ICoreBddRunner) runner;
        }
    }
}
