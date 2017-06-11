using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class ScenarioRunner : IScenarioRunner
    {
        private readonly ScenarioExecutor _scenarioExecutor;
        private readonly IMetadataProvider _metadataProvider;
        private readonly IScenarioProgressNotifier _progressNotifier;
        private IEnumerable<StepDescriptor> _steps = Enumerable.Empty<StepDescriptor>();
        private INameInfo _name;
        private string[] _labels = Arrays<string>.Empty();
        private string[] _categories = Arrays<string>.Empty();
        private Func<object> _contextProvider = ProvideNoContext;
        private readonly ExceptionProcessor _exceptionProcessor;

        public ScenarioRunner(ScenarioExecutor scenarioExecutor, IMetadataProvider metadataProvider, IScenarioProgressNotifier progressNotifier, ExceptionProcessor exceptionProcessor)
        {
            _scenarioExecutor = scenarioExecutor;
            _metadataProvider = metadataProvider;
            _progressNotifier = progressNotifier;
            _exceptionProcessor = exceptionProcessor;
        }

        public IScenarioRunner WithSteps(IEnumerable<StepDescriptor> steps)
        {
            if (_steps == null)
                throw new ArgumentNullException(nameof(steps));
            _steps = steps;
            return this;
        }

        public IScenarioRunner WithCapturedScenarioDetails()
        {
            var scenario = _metadataProvider.CaptureCurrentScenario();
            return WithName(_metadataProvider.GetScenarioName(scenario))
                .WithLabels(_metadataProvider.GetScenarioLabels(scenario.MethodInfo))
                .WithCategories(_metadataProvider.GetScenarioCategories(scenario.MethodInfo));
        }

        public IScenarioRunner WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Unable to create scenario without name", nameof(name));
            _name = new NameInfo(name, Arrays<INameParameterInfo>.Empty());
            return this;
        }

        public IScenarioRunner WithContext(Func<object> contextProvider)
        {
            if (contextProvider == null)
                throw new ArgumentNullException(nameof(contextProvider));
            _contextProvider = contextProvider;
            return this;
        }

        private IScenarioRunner WithName(INameInfo name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            _name = name;
            return this;
        }

        private void Validate()
        {
            if (_name == null)
                throw new InvalidOperationException("Scenario name is not provided.");
            if (!_steps.Any())
                throw new InvalidOperationException("At least one step has to be provided");
        }

        public IScenarioRunner WithLabels(string[] labels)
        {
            if (labels == null)
                throw new ArgumentNullException(nameof(labels));
            _labels = labels;
            return this;
        }

        public IScenarioRunner WithCategories(string[] categories)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));
            _categories = categories;
            return this;
        }

        public Task RunAsynchronously()
        {
            Validate();
            return _scenarioExecutor.ExecuteAsync(new ScenarioInfo(_name, _labels, _categories), ProvideSteps, _contextProvider, _progressNotifier);
        }

        public void RunSynchronously()
        {
            var task = RunAsynchronously();
            if (!task.IsCompleted)
                throw new InvalidOperationException("Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.");
            task.GetAwaiter().GetResult();
        }

        private RunnableStep[] ProvideSteps(ExtendableExecutor extendableExecutor, object scenarioContext)
        {
            return ProvideSteps(extendableExecutor, scenarioContext, _steps.ToArray(), string.Empty);
        }

        private RunnableStep[] ProvideSteps(ExtendableExecutor extendableExecutor, object scenarioContext, StepDescriptor[] steps, string groupPrefix)
        {
            var totalStepsCount = steps.Length;
            string previousStepTypeName = null;
            var result = new RunnableStep[totalStepsCount];

            for (int i = 0; i < totalStepsCount; ++i)
            {
                var step = ToRunnableStep(steps[i], i, totalStepsCount, previousStepTypeName, extendableExecutor, scenarioContext, groupPrefix);
                result[i] = step;
                previousStepTypeName = step.Result.Info.Name.StepTypeName?.OriginalName;
            }

            return result;
        }

        private RunnableStep ToRunnableStep(StepDescriptor descriptor, int stepIndex, int totalStepsCount, string previousStepTypeName, ExtendableExecutor extendableExecutor, object scenarioContext, string groupPrefix)
        {
            var stepInfo = new StepInfo(_metadataProvider.GetStepName(descriptor, previousStepTypeName), stepIndex + 1, totalStepsCount, groupPrefix);
            var arguments = descriptor.Parameters.Select(p => new MethodArgument(p, _metadataProvider.GetParameterFormatter(p.ParameterInfo))).ToArray();
            var stepGroupPrefix = $"{stepInfo.GroupPrefix}{stepInfo.Number}.";
            return new RunnableStep(stepInfo, TransformInvocationResult(descriptor, extendableExecutor, stepGroupPrefix), arguments, _exceptionProcessor, _progressNotifier, extendableExecutor, scenarioContext);
        }

        private Func<object, object[], Task<RunnableStepResult>> TransformInvocationResult(StepDescriptor descriptor, ExtendableExecutor extendableExecutor, string groupPrefix)
        {
            var invocation = descriptor.StepInvocation;

            async Task<RunnableStepResult> Invoke(object context, object[] args)
            {
                var result = await invocation.Invoke(context, args);
                var subStepsContext = InstantiateSubStepsContext(result);
                try
                {
                    return new RunnableStepResult(ProvideSteps(extendableExecutor, subStepsContext, result.SubSteps.ToArray(), groupPrefix));
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Sub-steps initialization failed: {e.Message}", e);
                }
            }
            return Invoke;
        }

        private static object InstantiateSubStepsContext(StepResultDescriptor result)
        {
            try
            {
                return result.SubStepsContextProvider.Invoke();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Sub-steps context initialization failed: {e.Message}", e);
            }
        }

        private static object ProvideNoContext()
        {
            return null;
        }
    }
}