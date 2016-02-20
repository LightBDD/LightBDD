using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    public abstract class CoreBddRunner : IBddRunner, ICoreBddRunner, IDisposable
    {
        public IIntegrationContext IntegrationContext { get; }
        private readonly FeatureResult _featureResult;
        private readonly ScenarioExecutor _scenarioExecutor;
        private bool _disposed;

        protected CoreBddRunner(Type featureType, IIntegrationContext integrationContext)
        {
            IntegrationContext = integrationContext;
            _featureResult = new FeatureResult(IntegrationContext.MetadataProvider.GetFeatureInfo(featureType));

            _scenarioExecutor = new ScenarioExecutor(IntegrationContext.ProgressNotifier);
            _scenarioExecutor.ScenarioExecuted += _featureResult.AddScenario;

            integrationContext.ProgressNotifier.NotifyFeatureStart(_featureResult.Info);
        }

        public IFeatureResult GetFeatureResult()
        {
            VerifyDisposed();
            return _featureResult;
        }

        public IScenarioRunner NewScenario()
        {
            VerifyDisposed();
            return new ScenarioRunner(_scenarioExecutor, IntegrationContext.MetadataProvider, IntegrationContext.ProgressNotifier, IntegrationContext.ExceptionToStatusMapper);
        }

        private void VerifyDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(null, "Runner is already disposed.");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            IntegrationContext.ProgressNotifier.NotifyFeatureFinished(_featureResult);
        }
    }
}