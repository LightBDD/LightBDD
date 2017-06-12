using System;
using System.Diagnostics;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// <see cref="IStepGroupBuilder{TContext}"/> extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class StepGroupBuilderExtensions
    {
        /// <summary>
        /// Method allowing to retrieve the <see cref="IIntegrableStepGroupBuilder"/> instance allowing to define steps.
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