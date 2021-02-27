using System;
using System.Threading;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Framework.Notification.Implementation
{
    //TODO: remove in LightBDD 4.x
    class NotificationAdapter : IProgressNotifier
    {
        private readonly IFeatureProgressNotifier _featureProgressNotifier;
        private readonly Func<object, IScenarioProgressNotifier> _scenarioProgressNotifierProvider;
        private readonly AsyncLocal<IScenarioProgressNotifier> _scenarioNotifier = new AsyncLocal<IScenarioProgressNotifier>();

        public NotificationAdapter(IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifierProvider)
        {
            _featureProgressNotifier = featureProgressNotifier;
            _scenarioProgressNotifierProvider = scenarioProgressNotifierProvider;
        }

        public void Notify(ProgressEvent e)
        {
            switch (e)
            {
                case FeatureStarting fs:
                    _featureProgressNotifier.NotifyFeatureStart(fs.Feature);
                    break;
                case FeatureFinished ff:
                    _featureProgressNotifier.NotifyFeatureFinished(ff.Result);
                    break;
                case ScenarioStarting ss:
                    HandleScenarioStarted(ss);
                    break;
                case ScenarioFinished sf:
                    HandleScenarioFinished(sf);
                    break;
                case StepStarting sts:
                    _scenarioNotifier.Value.NotifyStepStart(sts.Step);
                    break;
                case StepFinished stf:
                    _scenarioNotifier.Value.NotifyStepFinished(stf.Result);
                    break;
                case StepCommented sc:
                    _scenarioNotifier.Value.NotifyStepComment(sc.Step, sc.Comment);
                    break;
            }
        }

        private void HandleScenarioStarted(ScenarioStarting scenarioStarting)
        {
            _scenarioNotifier.Value = _scenarioProgressNotifierProvider.Invoke(ScenarioExecutionContext.CurrentScenarioFixture);
            _scenarioNotifier.Value.NotifyScenarioStart(scenarioStarting.Scenario);
        }

        private void HandleScenarioFinished(ScenarioFinished scenarioFinished)
        {
            _scenarioNotifier.Value.NotifyScenarioFinished(scenarioFinished.Result);
            _scenarioNotifier.Value = null;
        }
    }
}