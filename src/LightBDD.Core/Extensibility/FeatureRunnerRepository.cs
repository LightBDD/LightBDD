using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LightBDD.Core.Extensibility.Implementation;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Abstract runner factory allowing to create <see cref="IFeatureRunner"/> instances and maintaining list of instantiated runners.
    /// This class should be used by projects integrating LightBDD with testing frameworks.
    /// </summary>
    public abstract class FeatureRunnerRepository
    {
        internal IntegrationContext IntegrationContext { get; }
        private readonly ConcurrentDictionary<Type, IFeatureRunner> _runners = new ConcurrentDictionary<Type, IFeatureRunner>();

        /// <summary>
        /// Constructor instantiating factory with specified integration context.
        /// </summary>
        /// <param name="integrationContext">Integration context.</param>
        [Obsolete("Use constructor with " + nameof(Extensibility.IntegrationContext), true)]
        protected FeatureRunnerRepository(IIntegrationContext integrationContext)
        {
            IntegrationContext = integrationContext as IntegrationContext ?? new IntegrationContextWrapper(integrationContext);
        }
        /// <summary>
        /// Constructor instantiating factory with specified runner context.
        /// </summary>
        /// <param name="integrationContext">Runner context.</param>
        protected FeatureRunnerRepository(IntegrationContext integrationContext)
        {
            IntegrationContext = integrationContext;
        }

        /// <summary>
        /// Returns feature runner for specified feature type.
        /// If runner already exists for <paramref name="featureType"/>, the existing instance is returned, otherwise a new instance is being created.
        /// </summary>
        /// <param name="featureType">Type of class describing feature and holding list of scenario methods.</param>
        /// <returns>Feature runner.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="featureType"/> is null.</exception>
        public IFeatureRunner GetRunnerFor(Type featureType)
        {
            if (featureType == null)
                throw new ArgumentNullException(nameof(featureType));

            return _runners.GetOrAdd(featureType, type => new FeatureRunner(type, IntegrationContext));
        }

        /// <summary>
        /// All currently instantiated runners.
        /// </summary>
        public IEnumerable<IFeatureRunner> AllRunners => _runners.Values;

    }
}