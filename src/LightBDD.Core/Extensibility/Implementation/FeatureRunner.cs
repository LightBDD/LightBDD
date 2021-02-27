using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;
using System;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class FeatureRunner : IFeatureRunner
    {
        private readonly FeatureResult _featureResult;
        private readonly IntegrationContext _integrationContext;
        private readonly IExecutionTimer _executionTimer;
        private readonly Type _featureType;
        private bool _disposed;
        private readonly ExceptionProcessor _exceptionProcessor;

        public FeatureRunner(Type featureType, IntegrationContext integrationContext, IExecutionTimer executionTimer)
        {
            _featureType = featureType;
            _integrationContext = integrationContext;
            _executionTimer = executionTimer;
            _exceptionProcessor = new ExceptionProcessor(_integrationContext);
            _featureResult = new FeatureResult(_integrationContext.MetadataProvider.GetFeatureInfo(featureType));

            integrationContext.ProgressNotifier.Notify(new FeatureStarting(executionTimer.GetTime(), _featureResult.Info));
        }

        public IFeatureFixtureRunner ForFixture(object fixture)
        {
            VerifyDisposed();

            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));
            if (fixture.GetType() != _featureType)
                throw new ArgumentException($"Provided fixture instance '{fixture.GetType()}' type does not match feature type '{_featureType}'");

            return new FeatureFixtureRunner(fixture, CreateScenarioRunner);
        }

        private ICoreScenarioBuilder CreateScenarioRunner(object fixture)
        {
            VerifyDisposed();
            return new ScenarioBuilder(_featureResult.Info, fixture, _integrationContext, _exceptionProcessor, _featureResult.AddScenario, _executionTimer);
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
            _integrationContext.ProgressNotifier.Notify(new FeatureFinished(_executionTimer.GetTime(), _featureResult));
        }
        public IFeatureResult GetFeatureResult()
        {
            return _featureResult;
        }
    }
}