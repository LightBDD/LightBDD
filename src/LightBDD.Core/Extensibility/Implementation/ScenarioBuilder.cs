#nullable enable
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class ScenarioBuilder : ICoreScenarioBuilder
    {
        private readonly IFeatureInfo _featureInfo;
        private readonly RunnableScenarioContext _context;
        private INameInfo? _name;
        private string[] _labels = Array.Empty<string>();
        private string[] _categories = Array.Empty<string>();
        private IEnumerable<StepDescriptor> _steps = Enumerable.Empty<StepDescriptor>();
        private ExecutionContextDescriptor _contextDescriptor = ExecutionContextDescriptor.NoContext;
        private IEnumerable<IScenarioDecorator> _scenarioDecorators = Enumerable.Empty<IScenarioDecorator>();
        private IScenarioInfo? _scenarioInfo;
        private string? _runtimeId;

        public ScenarioBuilder(IFeatureInfo featureInfo, object fixture, IntegrationContext integrationContext,
            ExceptionProcessor exceptionProcessor, Action<IScenarioResult> onScenarioFinished)
        {
            _featureInfo = featureInfo;
            _context = new RunnableScenarioContext(
                integrationContext,
                exceptionProcessor,
                onScenarioFinished,
                fixture,
                ProvideSteps);
        }

        public ICoreScenarioBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));
            _steps = _steps.Concat(steps);
            return this;
        }

        public ICoreScenarioBuilder WithCapturedScenarioDetails()
        {
            var metadataProvider = _context.IntegrationContext.MetadataProvider;
            var scenario = metadataProvider.CaptureCurrentScenario();
            return WithName(metadataProvider.GetScenarioName(scenario))
                .WithLabels(metadataProvider.GetScenarioLabels(scenario.MethodInfo))
                .WithCategories(metadataProvider.GetScenarioCategories(scenario.MethodInfo))
                .WithScenarioDecorators(metadataProvider.GetScenarioDecorators(scenario));
        }

        public ICoreScenarioBuilder WithCapturedScenarioDetailsIfNotSpecified()
        {
            var metadataProvider = _context.IntegrationContext.MetadataProvider;
            var scenario = metadataProvider.CaptureCurrentScenario();
            if (_name == null) WithName(metadataProvider.GetScenarioName(scenario));
            if (!_labels.Any()) WithLabels(metadataProvider.GetScenarioLabels(scenario.MethodInfo));
            if (!_categories.Any()) WithCategories(metadataProvider.GetScenarioCategories(scenario.MethodInfo));
            if (!_scenarioDecorators.Any()) WithScenarioDecorators(metadataProvider.GetScenarioDecorators(scenario));
            return this;
        }

        public ICoreScenarioBuilder WithLabels(string[] labels)
        {
            _labels = labels ?? throw new ArgumentNullException(nameof(labels));
            _scenarioInfo = null;
            return this;
        }

        public ICoreScenarioBuilder WithCategories(string[] categories)
        {
            _categories = categories ?? throw new ArgumentNullException(nameof(categories));
            _scenarioInfo = null;
            return this;
        }

        public ICoreScenarioBuilder WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Unable to create scenario without name", nameof(name));
            _name = new NameInfo(name, Array.Empty<INameParameterInfo>());
            _scenarioInfo = null;
            return this;
        }

        private ICoreScenarioBuilder WithName(INameInfo name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _scenarioInfo = null;
            return this;
        }

        public ICoreScenarioBuilder WithContext(Func<object> contextProvider, bool takeOwnership)
        {
            _contextDescriptor = new ExecutionContextDescriptor(contextProvider, takeOwnership);
            return this;
        }

        public ICoreScenarioBuilder WithContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator)
        {
            _contextDescriptor = new ExecutionContextDescriptor(contextProvider, scopeConfigurator);
            return this;
        }

        public ICoreScenarioBuilder WithScenarioDecorators(IEnumerable<IScenarioDecorator> scenarioDecorators)
        {
            _scenarioDecorators = scenarioDecorators ?? throw new ArgumentNullException(nameof(scenarioDecorators));
            return this;
        }

        public IRunnableScenario Build()
        {
            if (_name == null)
                throw new InvalidOperationException("Scenario name is not provided.");
            //TODO:review runtimeId
            var scenarioInfo = _scenarioInfo ?? new ScenarioInfo(_featureInfo, _name, _labels, _categories, _runtimeId ?? Guid.NewGuid().ToString());
            return new RunnableScenario(_context, scenarioInfo, _steps, _contextDescriptor, GetScenarioDecorators());
        }

        public LightBddConfiguration Configuration => _context.IntegrationContext.Configuration;
        public ICoreScenarioBuilder WithScenarioDetails(IScenarioInfo scenarioInfo)
        {
            _scenarioInfo = scenarioInfo;
            _name = _scenarioInfo.Name;
            _labels = scenarioInfo.Labels.ToArray();
            _categories = scenarioInfo.Categories.ToArray();
            _runtimeId = scenarioInfo.RuntimeId;
            return this;
        }

        public ICoreScenarioBuilder WithRuntimeId(string runtimeId)
        {
            _runtimeId = runtimeId;
            return this;
        }

        private IEnumerable<IScenarioDecorator> GetScenarioDecorators()
        {
            return _context.IntegrationContext.ExecutionExtensions.ScenarioDecorators.Concat(_scenarioDecorators);
        }

        private void ValidateContext()
        {
            if (_name == null)
                throw new InvalidOperationException("Scenario name is not provided.");
        }

        private RunnableStep[] ProvideSteps(IMetadataInfo parent, IEnumerable<StepDescriptor> stepDescriptors, object context, IDependencyContainer container, string groupPrefix, Func<Exception, bool> shouldAbortSubStepExecutionFn)
        {
            var descriptors = stepDescriptors.ToArray();
            if (!descriptors.Any())
                throw new InvalidOperationException("At least one step has to be provided");

            var metadataProvider = _context.IntegrationContext.MetadataProvider;

            var totalSteps = descriptors.Length;
            var steps = new RunnableStep[totalSteps];
            string? previousStepTypeName = null;

            var extensions = _context.IntegrationContext.ExecutionExtensions;
            var stepContext = new RunnableStepContext(_context.ExceptionProcessor, _context.ProgressNotifier, container, context, ProvideSteps, shouldAbortSubStepExecutionFn, _context.ExecutionTimer, _context.FileAttachmentsManager);
            for (var stepIndex = 0; stepIndex < totalSteps; ++stepIndex)
            {
                var descriptor = descriptors[stepIndex];
                var stepInfo = new StepInfo(parent, metadataProvider.GetStepName(descriptor, previousStepTypeName), stepIndex + 1, totalSteps, groupPrefix);
                var arguments = descriptor.Parameters.Select(p => new MethodArgument(p, metadataProvider.GetValueFormattingServiceFor(p.ParameterInfo))).ToArray();

                steps[stepIndex] = new RunnableStep(stepContext, stepInfo, descriptor, arguments, extensions.StepDecorators.Concat(metadataProvider.GetStepDecorators(descriptor)));
                previousStepTypeName = stepInfo.Name.StepTypeName?.OriginalName;
            }

            return steps;
        }
    }


}