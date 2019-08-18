using System;
using System.Collections.Generic;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal delegate RunnableStep[] ProvideStepsFunc(IMetadataInfo parent,IEnumerable<StepDescriptor> stepDescriptors, object context, IDependencyContainer container, string groupPrefix, Func<Exception, bool> shouldAbortSubStepExecutionFn);
    internal class RunnableScenarioContext
    {
        public IntegrationContext IntegrationContext { get; }
        public ExceptionProcessor ExceptionProcessor { get; }
        public Action<IScenarioResult> OnScenarioFinished { get; }
        public IScenarioProgressNotifier ProgressNotifier { get; }
        public ProvideStepsFunc StepsProvider { get; }

        public RunnableScenarioContext(IntegrationContext integrationContext,
            ExceptionProcessor exceptionProcessor,
            Action<IScenarioResult> onScenarioFinished,
            IScenarioProgressNotifier progressNotifier,
            ProvideStepsFunc stepsProvider)
        {
            IntegrationContext = integrationContext;
            ExceptionProcessor = exceptionProcessor;
            OnScenarioFinished = onScenarioFinished;
            ProgressNotifier = progressNotifier;
            StepsProvider = stepsProvider;
        }
    }
}