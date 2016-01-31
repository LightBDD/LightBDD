namespace LightBDD.Core.Extensibility
{
    public static class BddRunnerExtensions
    {
        public static ICoreBddRunner Integrate(this IBddRunner runner)
        {
            return (ICoreBddRunner) runner;
        }
    }
}
