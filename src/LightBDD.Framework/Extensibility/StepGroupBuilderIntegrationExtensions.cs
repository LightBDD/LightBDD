using System;
using System.Diagnostics;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// <see cref="IStepGroupBuilder{TContext}"/> integration extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class StepGroupBuilderIntegrationExtensions
    {
        /// <summary>
        /// Method allowing to retrieve the <see cref="IIntegrableStepGroupBuilder"/> instance allowing to add steps to composite step or scenario that is currently being defined.
        /// This method is dedicated for projects extending LightBDD with user friendly API for defining steps - it should not be used directly by regular LightBDD users.
        /// </summary>
        /// <typeparam name="TContext">Step context type.</typeparam>
        /// <param name="builder">Builder.</param>
        /// <returns>Instance of <see cref="IIntegrableStepGroupBuilder"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="builder"/> does not implement <see cref="IIntegrableStepGroupBuilder"/>.</exception>
        public static IIntegrableStepGroupBuilder Integrate<TContext>(this IStepGroupBuilder<TContext> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (!(builder is IIntegrableStepGroupBuilder))
                throw new NotSupportedException($"The type '{builder.GetType().Name}' has to implement '{nameof(IIntegrableStepGroupBuilder)}' interface to support integration.");

            return (IIntegrableStepGroupBuilder)builder;
        }
    }
}