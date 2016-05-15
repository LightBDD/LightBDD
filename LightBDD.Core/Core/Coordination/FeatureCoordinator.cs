using System;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.Coordination
{
    public abstract class FeatureCoordinator : IDisposable
    {
        private static readonly object Sync = new object();
        protected static FeatureCoordinator Instance { get; private set; }
        private readonly IFeatureAggregator _featureAggregator;
        public BddRunnerFactory RunnerFactory { get; }
        public bool IsDisposed { get; private set; }

        protected static void Install(FeatureCoordinator coordinator)
        {
            lock (Sync)
            {
                if (Instance != null)
                    throw new InvalidOperationException($"FeatureCoordinator of {Instance.GetType()} type is already installed");
                Instance = coordinator;
            }
        }

        protected FeatureCoordinator(BddRunnerFactory runnerFactory, IFeatureAggregator featureAggregator)
        {
            _featureAggregator = featureAggregator;
            RunnerFactory = runnerFactory;
        }

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
            foreach (var runner in RunnerFactory.AllRunners)
            {
                runner.Dispose();
                _featureAggregator.Aggregate(runner.GetFeatureResult());
            }
        }
    }
}
