using System;
using System.Collections.Generic;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal delegate RunnableStep[] ProvideStepsFunc(IMetadataInfo parent, IEnumerable<StepDescriptor> stepDescriptors, object context, IDependencyContainer container, string groupPrefix, Func<Exception, bool> shouldAbortSubStepExecutionFn);
    internal class RunnableScenarioContext
    {
        public IntegrationContext IntegrationContext { get; }
        public ExceptionProcessor ExceptionProcessor { get; }
        public Action<IScenarioResult> OnScenarioFinished { get; }
        public object FixtureObject { get; }
        public ProvideStepsFunc StepsProvider { get; }
        public IProgressNotifier ProgressNotifier => IntegrationContext.ProgressNotifier;
        public IExecutionTimer ExecutionTimer { get; }

        public RunnableScenarioContext(IntegrationContext integrationContext,
            ExceptionProcessor exceptionProcessor,
            Action<IScenarioResult> onScenarioFinished,
            object fixtureObject,
            ProvideStepsFunc stepsProvider,
            IExecutionTimer executionTimer)
        {
            IntegrationContext = integrationContext;
            ExceptionProcessor = exceptionProcessor;
            OnScenarioFinished = onScenarioFinished;
            FixtureObject = fixtureObject;
            StepsProvider = stepsProvider;
            ExecutionTimer = executionTimer;
        }
    }
}