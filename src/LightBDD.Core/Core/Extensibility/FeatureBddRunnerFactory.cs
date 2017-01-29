using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LightBDD.Core.Execution.Implementation;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Abstract runner factory allowing to create <see cref="IFeatureBddRunner"/> instances and maintaining list of instantiated runners.
    /// This class should be used by projects integrating LightBDD with testing frameworks.
    /// </summary>
    public abstract class FeatureBddRunnerFactory
    {
        private readonly IIntegrationContext _integrationContext;
        private readonly ConcurrentDictionary<Type, IFeatureBddRunner> _runners = new ConcurrentDictionary<Type, IFeatureBddRunner>();

        /// <summary>
        /// Constructor instantiating factory with specified integration context.
        /// </summary>
        /// <param name="integrationContext">Integration context.</param>
        protected FeatureBddRunnerFactory(IIntegrationContext integrationContext)
        {
            _integrationContext = integrationContext;
        }

        /// <summary>
        /// Returns feature runner for specified feature type.
        /// If runner already exists for <paramref name="featureType"/>, the existing instance is returned, otherwise a new instance is being created.
        /// </summary>
        /// <param name="featureType">Type of class describing feature and holding list of scenario methods.</param>
        /// <returns>Feature runner.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="featureType"/> is null.</exception>
        public IFeatureBddRunner GetRunnerFor(Type featureType)
        {
            if (featureType == null)
                throw new ArgumentNullException(nameof(featureType));

            return _runners.GetOrAdd(featureType, type => new FeatureBddRunner(type, _integrationContext));
        }

        /// <summary>
        /// All currently instantiated runners.
        /// </summary>
        public IEnumerable<IFeatureBddRunner> AllRunners => _runners.Values;

    }
}