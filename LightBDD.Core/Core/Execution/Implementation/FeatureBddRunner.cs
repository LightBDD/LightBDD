using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    internal class FeatureBddRunner : IFeatureBddRunner
    {
        private readonly FeatureResult _featureResult;
        private readonly ScenarioExecutor _scenarioExecutor;
        private readonly IIntegrationContext _integrationContext;
        private readonly Type _featureType;
        private bool _disposed;

        public FeatureBddRunner(Type featureType, IIntegrationContext integrationContext)
        {
            _featureType = featureType;
            _integrationContext = integrationContext;
            _featureResult = new FeatureResult(_integrationContext.MetadataProvider.GetFeatureInfo(featureType));

            _scenarioExecutor = new ScenarioExecutor();
            _scenarioExecutor.ScenarioExecuted += _featureResult.AddScenario;

            integrationContext.FeatureProgressNotifier.NotifyFeatureStart(_featureResult.Info);
        }

        public IBddRunner GetRunner(object fixture)
        {
            VerifyDisposed();

            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));
            if (fixture.GetType() != _featureType)
                throw new ArgumentException($"Provided fixture instance '{fixture.GetType()}' type does not match feature type '{_featureType}'");

            return new CoreBddRunner(() => CreateScenarioRunner(fixture));
        }

        private IScenarioRunner CreateScenarioRunner(object fixture)
        {
            VerifyDisposed();
            return new ScenarioRunner(_scenarioExecutor, _integrationContext.MetadataProvider, _integrationContext.ScenarioProgressNotifierProvider.Invoke(fixture), _integrationContext.ExceptionToStatusMapper);
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
            _integrationContext.FeatureProgressNotifier.NotifyFeatureFinished(_featureResult);
        }
        public IFeatureResult GetFeatureResult()
        {
            return _featureResult;
        }
    }
}