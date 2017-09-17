using System;
using System.Diagnostics;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// <see cref="ICompositeStepBuilder{TContext}"/> integration extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class CompositeStepBuilderIntegrationExtensions
    {
        /// <summary>
        /// Method allowing to retrieve the <see cref="IIntegrableCompositeStepBuilder"/> instance allowing to add steps to composite step or scenario that is currently being defined.
        /// This method is dedicated for projects extending LightBDD with user friendly API for defining steps - it should not be used directly by regular LightBDD users.
        /// </summary>
        /// <typeparam name="TContext">Step context type.</typeparam>
        /// <param name="builder">Builder.</param>
        /// <returns>Instance of <see cref="IIntegrableCompositeStepBuilder"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="builder"/> does not implement <see cref="IIntegrableCompositeStepBuilder"/>.</exception>
        public static IIntegrableCompositeStepBuilder Integrate<TContext>(this ICompositeStepBuilder<TContext> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (!(builder is IIntegrableCompositeStepBuilder))
                throw new NotSupportedException($"The type '{builder.GetType().Name}' has to implement '{nameof(IIntegrableCompositeStepBuilder)}' interface to support integration.");

            return (IIntegrableCompositeStepBuilder)builder;
        }
    }
}