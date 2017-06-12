using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// Interface allowing to build step group from list of sub-steps described by <see cref="StepDescriptor"/> instances and step context.
    /// The interface is dedicated for projects extending LightBDD with user friendly API for defining step groups - it should not be used directly by regular LightBDD users. 
    /// </summary>
    public interface IIntegrableStepGroupBuilder
    {
        /// <summary>
        /// Adds <paramref name="steps"/> to the sub-step collection.
        /// </summary>
        /// <param name="steps">Steps to add.</param>
        /// <returns>Self.</returns>
        IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps);
        /// <summary>
        /// Uses <paramref name="contextProvider"/> to instantiate context that would be shared with all sub-steps.
        /// Please note that context can be specified only once and only when there is no steps added yet.
        /// </summary>
        /// <param name="contextProvider">Context provider function.</param>
        /// <returns>Self.</returns>
        /// <exception cref="InvalidOperationException">Thrown if context is already specified or if some steps has been already added.</exception>
        IIntegrableStepGroupBuilder WithStepContext(Func<object> contextProvider);
        /// <summary>
        /// Creates enriched runner based on <see cref="IIntegrableStepGroupBuilder"/> and <see cref="LightBddConfiguration"/>.
        /// </summary>
        /// <typeparam name="TEnrichedStepGroupBuilder">Type of enriched builder.</typeparam>
        /// <param name="builderFactory">Builder factory.</param>
        /// <returns>Enriched builder instance.</returns>
        TEnrichedStepGroupBuilder Enrich<TEnrichedStepGroupBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedStepGroupBuilder> builderFactory);
        /// <summary>
        /// Builds <see cref="StepGroup"/> based on specified steps and step context provider.
        /// </summary>
        /// <returns><see cref="StepGroup"/> instance.</returns>
        StepGroup Build();
    }
}