using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class ScenarioRunnerV2 : IScenarioRunner
    {
        private readonly RunnableScenarioContext _context;
        private INameInfo _name;
        private string[] _labels = Arrays<string>.Empty();
        private string[] _categories = Arrays<string>.Empty();
        private IEnumerable<StepDescriptor> _steps = Enumerable.Empty<StepDescriptor>();
        private ExecutionContextDescriptor _contextDescriptor = ExecutionContextDescriptor.NoContext;

        public ScenarioRunnerV2(object fixture, IntegrationContext integrationContext, ExceptionProcessor exceptionProcessor, Action<IScenarioResult> onScenarioFinished)
        {
            _context = new RunnableScenarioContext(
                integrationContext,
                exceptionProcessor,
                onScenarioFinished,
                integrationContext.ScenarioProgressNotifierProvider.Invoke(fixture),
                ProvideSteps);
        }

        public IScenarioRunner WithSteps(IEnumerable<StepDescriptor> steps)
        {
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            return this;
        }

        public IScenarioRunner WithCapturedScenarioDetails()
        {
            var metadataProvider = _context.IntegrationContext.MetadataProvider;
            var scenario = metadataProvider.CaptureCurrentScenario();
            return WithName(metadataProvider.GetScenarioName(scenario))
                .WithLabels(metadataProvider.GetScenarioLabels(scenario.MethodInfo))
                .WithCategories(metadataProvider.GetScenarioCategories(scenario.MethodInfo))
                .WithScenarioDecorators(metadataProvider.GetScenarioDecorators(scenario));
        }

        public IScenarioRunner WithLabels(string[] labels)
        {
            _labels = labels ?? throw new ArgumentNullException(nameof(labels));
            return this;
        }

        public IScenarioRunner WithCategories(string[] categories)
        {
            _categories = categories ?? throw new ArgumentNullException(nameof(categories));
            return this;
        }

        public IScenarioRunner WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Unable to create scenario without name", nameof(name));
            _name = new NameInfo(name, Arrays<INameParameterInfo>.Empty());
            return this;
        }

        private IScenarioRunner WithName(INameInfo name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }

        public IScenarioRunner WithContext(Func<object> contextProvider, bool takeOwnership)
        {
            _contextDescriptor = new ExecutionContextDescriptor(contextProvider, takeOwnership);
            return this;
        }

        public IScenarioRunner WithContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator)
        {
            _contextDescriptor = new ExecutionContextDescriptor(contextProvider, scopeConfigurator);
            return this;
        }

        public IScenarioRunner WithScenarioDecorators(IEnumerable<IScenarioDecorator> scenarioDecorators)
        {
            return this;
        }

        public void RunScenario()
        {
            var task = RunScenarioAsync();
            if (!task.IsCompleted)
                throw new InvalidOperationException("Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.");
            task.GetAwaiter().GetResult();
        }

        public Task RunScenarioAsync()
        {
            ValidateContext();
            return new RunnableScenarioV2(_context, new ScenarioInfo(_name, _labels, _categories), _steps, _contextDescriptor).ExecuteAsync();
        }

        private void ValidateContext()
        {
            if (_name == null)
                throw new InvalidOperationException("Scenario name is not provided.");
        }

        private RunnableStepV2[] ProvideSteps(IEnumerable<StepDescriptor> stepDescriptors, object context, IDependencyContainer container, string groupPrefix, Func<Exception, bool> shouldAbortSubStepExecutionFn)
        {
            var descriptors = stepDescriptors.ToArray();
            if (!descriptors.Any())
                throw new InvalidOperationException("At least one step has to be provided");

            var metadataProvider = _context.IntegrationContext.MetadataProvider;

            var totalSteps = descriptors.Length;
            var steps = new RunnableStepV2[totalSteps];
            string previousStepTypeName = null;

            var stepContext = new RunnableStepContext(_context.ExceptionProcessor, _context.ProgressNotifier, container, context, ProvideSteps, shouldAbortSubStepExecutionFn);
            for (var stepIndex = 0; stepIndex < totalSteps; ++stepIndex)
            {
                var descriptor = descriptors[stepIndex];
                var stepInfo = new StepInfo(metadataProvider.GetStepName(descriptor, previousStepTypeName), stepIndex + 1, totalSteps, groupPrefix);
                var arguments = descriptor.Parameters.Select(p => new MethodArgument(p, metadataProvider.GetValueFormattingServiceFor(p.ParameterInfo))).ToArray();

                steps[stepIndex] = new RunnableStepV2(stepContext, stepInfo, descriptor.StepInvocation, arguments);
                previousStepTypeName = stepInfo.Name.StepTypeName?.OriginalName;
            }

            return steps;
        }
    }
}