using System;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.Execution.Coordination
{
    /// <summary>
    /// Feature coordinator singleton class holding <see cref="FeatureRunnerRepository"/> allowing to instantiate runners as well as <see cref="IFeatureAggregator"/> used for aggregate execution results on coordinator disposal.
    /// </summary>
    public abstract class FeatureCoordinator : IDisposable
    {
        private static readonly object Sync = new object();
        /// <summary>
        /// Feature coordinator instance.
        /// </summary>
        protected static FeatureCoordinator Instance { get; private set; }
        private readonly IFeatureAggregator _featureAggregator;
        /// <summary>
        /// Runner factory.
        /// </summary>
        public FeatureRunnerRepository RunnerRepository { get; }
        /// <summary>
        /// Returns <c>true</c> if already disposed, otherwise <c>false</c>.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Installs the specified feature coordinator in thread safe manner.
        /// </summary>
        /// <param name="coordinator">Coordinator instance to install.</param>
        /// <exception cref="InvalidOperationException">Exception thrown if another coordinator is already installed.</exception>
        protected static void Install(FeatureCoordinator coordinator)
        {
            lock (Sync)
            {
                if (Instance != null)
                    throw new InvalidOperationException($"FeatureCoordinator of {Instance.GetType()} type is already installed");
                Instance = coordinator;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="runnerRepository">Runner factory instance that would be used for instantiating runners.</param>
        /// <param name="featureAggregator">Feature aggregator instance used for aggregating feature results on coordinator disposal.</param>
        protected FeatureCoordinator(FeatureRunnerRepository runnerRepository, IFeatureAggregator featureAggregator)
        {
            _featureAggregator = featureAggregator;
            RunnerRepository = runnerRepository;
        }

        /// <summary>
        /// Disposes coordinator, triggering feature result aggregation.
        /// Each runner belonging to <see cref="RunnerRepository"/>, is disposed and its feature result is aggregated.
        /// After aggregation of all results, the feature aggregator is disposed as well.
        /// 
        /// If coordinator is already disposed, methods does nothing.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            CollectFeatureResults();
            _featureAggregator.Dispose();
        }

        private void CollectFeatureResults()
        {
            foreach (var runner in RunnerRepository.AllRunners)
            {
                runner.Dispose();
                _featureAggregator.Aggregate(runner.GetFeatureResult());
            }
        }
    }
}
