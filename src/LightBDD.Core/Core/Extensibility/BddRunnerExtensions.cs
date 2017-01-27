using System;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// IBddRunner extensions.
    /// </summary>
    public static class BddRunnerExtensions
    {
        /// <summary>
        /// Method allowing to retrieve the <see cref="ICoreBddRunner"/> instance allowing to define and execute scenarios.
        /// This method is dedicated for projects extending LightBDD with user friendly API for running scenarios - it should not be used directly by regular LightBDD users.
        /// </summary>
        /// <typeparam name="TContext">Bdd runner context type.</typeparam>
        /// <param name="runner">Bdd runner.</param>
        /// <returns>Instance of <see cref="ICoreBddRunner"/>.</returns>
        public static ICoreBddRunner Integrate<TContext>(this IBddRunner<TContext> runner)
        {
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));

            if (!(runner is ICoreBddRunner))
                throw new InvalidOperationException($"The type '{runner.GetType().Name}' has to implement '{nameof(ICoreBddRunner)}' interface to support integration.");

            return (ICoreBddRunner)runner;
        }
    }
}
