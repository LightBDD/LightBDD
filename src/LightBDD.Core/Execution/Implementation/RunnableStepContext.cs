using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Notification;
using LightBDD.Core.Reporting;

namespace LightBDD.Core.Execution.Implementation
{
    internal class RunnableStepContext
    {
        public RunnableStepContext(ExceptionProcessor exceptionProcessor, IProgressNotifier progressNotifier,
            IDependencyContainer container, object context, ProvideStepsFunc provideSteps,
            Func<Exception, bool> shouldAbortSubStepExecution, IExecutionTimer executionTimer, IFileAttachmentsManager fileAttachmentsManager)
        {
            ExceptionProcessor = exceptionProcessor;
            ProgressNotifier = progressNotifier;
            Container = container;
            Context = context;
            ProvideSteps = provideSteps;
            ShouldAbortSubStepExecution = shouldAbortSubStepExecution;
            ExecutionTimer = executionTimer;
            FileAttachmentsManager = fileAttachmentsManager;
        }

        public ExceptionProcessor ExceptionProcessor { get; }
        public IProgressNotifier ProgressNotifier { get; }
        public IDependencyContainer Container { get; }
        public object Context { get; }
        public ProvideStepsFunc ProvideSteps { get; }
        public Func<Exception, bool> ShouldAbortSubStepExecution { get; }
        public IExecutionTimer ExecutionTimer { get; }
        public IFileAttachmentsManager FileAttachmentsManager { get; }
    }
}