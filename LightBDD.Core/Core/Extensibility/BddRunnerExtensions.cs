using System;

namespace LightBDD.Core.Extensibility
{
    public static class BddRunnerExtensions
    {
        public static ICoreBddRunner Integrate<T>(this IBddRunner<T> runner)
        {
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));

            if (!(runner is ICoreBddRunner))
                throw new InvalidOperationException($"The type '{runner.GetType().Name}' has to implement '{nameof(ICoreBddRunner)}' interface to support integration.");

            return (ICoreBddRunner)runner;
        }
    }
}
