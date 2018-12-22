using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;
using System;
using System.Diagnostics;

namespace LightBDD.Core.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class FeatureRunner : IFeatureRunner
    {
        private readonly FeatureResult _featureResult;
        private readonly IntegrationContext _integrationContext;
        private readonly Type _featureType;
        private bool _disposed;
        private readonly ExceptionProcessor _exceptionProcessor;

        public FeatureRunner(Type featureType, IntegrationContext integrationContext)
        {
            _featureType = featureType;
            _integrationContext = integrationContext;
            _exceptionProcessor = new ExceptionProcessor(_integrationContext);
            _featureResult = new FeatureResult(_integrationContext.MetadataProvider.GetFeatureInfo(featureType));
            integrationContext.FeatureProgressNotifier.NotifyFeatureStart(_featureResult.Info);
        }

        public IFeatureFixtureRunner ForFixture(object fixture)
        {
            VerifyDisposed();

            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));
            if (fixture.GetType() != _featureType)
                throw new ArgumentException($"Provided fixture instance '{fixture.GetType()}' type does not match feature type '{_featureType}'");

            return new FeatureFixtureRunner(fixture, CreateScenarioRunner, _integrationContext.Configuration);
        }

        private IScenarioRunner CreateScenarioRunner(object fixture)
        {
            VerifyDisposed();
            return new ScenarioRunnerV2(fixture, _integrationContext, _exceptionProcessor, _featureResult.AddScenario);
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