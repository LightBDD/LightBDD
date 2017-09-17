using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// Interface allowing to configure step group such as scenario or composite step from list of sub-steps described by <see cref="StepDescriptor"/> instances.
    /// The interface is dedicated for projects extending LightBDD with user friendly API for defining composite steps - it should not be used directly by regular LightBDD users. 
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
        /// Creates enriched runner based on <see cref="IIntegrableStepGroupBuilder"/> and <see cref="LightBddConfiguration"/>.
        /// </summary>
        /// <typeparam name="TEnrichedBuilder">Type of enriched builder.</typeparam>
        /// <param name="builderFactory">Builder factory.</param>
        /// <returns>Enriched builder instance.</returns>
        TEnrichedBuilder Enrich<TEnrichedBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedBuilder> builderFactory);
    }
}