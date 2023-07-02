using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility.Implementation;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Funner factory allowing to create <see cref="IFeatureRunner"/> instances and maintaining list of instantiated runners.
    /// This class should be used by projects integrating LightBDD with testing frameworks.
    /// </summary>
    public class FeatureRunnerRepository : IDisposable
    {
        private readonly IntegrationContext _integrationContext;
        private readonly ConcurrentDictionary<Type, Lazy<IFeatureRunner>> _runners = new();

        /// <summary>
        /// Constructor instantiating factory with specified runner context.
        /// </summary>
        /// <param name="integrationContext">Runner context.</param>
        public FeatureRunnerRepository(IntegrationContext integrationContext)
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
        public IFeatureRunner GetRunnerFor(Type featureType)
        {
            if (featureType == null)
                throw new ArgumentNullException(nameof(featureType));

            var lazyRunnerFor = _runners.GetOrAdd(
                featureType,
                type => new Lazy<IFeatureRunner>(() => new FeatureRunner(type, _integrationContext), LazyThreadSafetyMode.ExecutionAndPublication));

            return lazyRunnerFor.Value;
        }

        /// <summary>
        /// All currently instantiated runners.
        /// </summary>
        public IEnumerable<IFeatureRunner> AllRunners => _runners.Values.Select(x => x.Value);

        /// <inheritdoc />
        public void Dispose()
        {
            var container = _integrationContext.DependencyContainer;
            try
            {
                // ReSharper disable AccessToDisposedClosure
                var globalSetUp = _integrationContext.Configuration.Get<ExecutionExtensionsConfiguration>().GlobalSetUpRegistry;
                Task.Run(() => globalSetUp.TearDownAsync(container)).GetAwaiter().GetResult();
                // ReSharper restore AccessToDisposedClosure
            }
            finally
            {
                container.Dispose();
            }
        }

        internal void Initialize()
        {
            var container = _integrationContext.DependencyContainer;
            var globalSetUp = _integrationContext.Configuration.Get<ExecutionExtensionsConfiguration>().GlobalSetUpRegistry;
            Task.Run(() => globalSetUp.SetUpAsync(container)).GetAwaiter().GetResult();
        }
    }
}