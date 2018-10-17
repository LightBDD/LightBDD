using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    //TODO: rework in 3.x
    internal class ScenarioExecutor
    {
        private readonly DecoratingExecutor _decoratingExecutor;
        [DebuggerStepThrough]
        public ScenarioExecutor(DecoratingExecutor decoratingExecutor)
        {
            _decoratingExecutor = decoratingExecutor;
        }

        [DebuggerStepThrough]
        public Task ExecuteAsync(ScenarioInfo scenario,
            Func<DecoratingExecutor, object, IDependencyContainer, RunnableStep[]> stepsProvider,
            ExecutionContextDescriptor contextDescriptor, IScenarioProgressNotifier progressNotifier,
            IEnumerable<IScenarioDecorator> scenarioDecorators, ExceptionProcessor exceptionProcessor,
            IDependencyContainer container, 
            IEnumerable<Func<object, Task>> setupFunctions,
            IEnumerable<Func<object, Task>> tearDownFunctions)
        {
            var runnableScenario = new RunnableScenario(scenario, stepsProvider, 
                contextDescriptor, progressNotifier,
                _decoratingExecutor, scenarioDecorators, 
                exceptionProcessor, container,
                setupFunctions,
                tearDownFunctions);
            try
            {
                return runnableScenario.RunAsync();
            }
            finally
            {
                ScenarioExecuted?.Invoke(runnableScenario.Result);
            }
        }

        public event Action<IScenarioResult> ScenarioExecuted;
    }
}