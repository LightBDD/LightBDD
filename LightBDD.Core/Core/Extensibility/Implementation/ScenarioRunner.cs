using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Helpers;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class ScenarioRunner : IScenarioRunner
    {
        private readonly ScenarioExecutor _scenarioExecutor;
        private readonly IMetadataProvider _metadataProvider;
        private readonly IProgressNotifier _progressNotifier;
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        private IEnumerable<StepDescriptor> _steps = Enumerable.Empty<StepDescriptor>();
        private INameInfo _name;
        private string[] _labels = Arrays<string>.Empty();
        private string[] _categories = Arrays<string>.Empty();
        private Func<object> _contextProvider = () => null;

        public ScenarioRunner(ScenarioExecutor scenarioExecutor, IMetadataProvider metadataProvider, IProgressNotifier progressNotifier, Func<Exception, ExecutionStatus> exceptionToStatusMapper)
        {
            _scenarioExecutor = scenarioExecutor;
            _metadataProvider = metadataProvider;
            _progressNotifier = progressNotifier;
            _exceptionToStatusMapper = exceptionToStatusMapper;
        }

        public IScenarioRunner WithSteps(IEnumerable<StepDescriptor> steps)
        {
            _steps = steps;
            return this;
        }

        public IScenarioRunner WithCapturedScenarioDetails()
        {
            var methodInfo = _metadataProvider.CaptureCurrentScenarioMethod();

            return WithName(_metadataProvider.GetScenarioName(methodInfo))
                .WithLabels(_metadataProvider.GetScenarioLabels(methodInfo))
                .WithCategories(_metadataProvider.GetScenarioCategories(methodInfo));
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
                throw new ArgumentNullException("Name", "Scenario name is not provided.");
            if (!_steps.Any())
                throw new ArgumentException("At least one step has to be provided", "Steps");
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
            return _scenarioExecutor.Execute(new ScenarioInfo(_name, _labels, _categories), ProvideSteps, _contextProvider);
        }

        public void RunSynchronously()
        {
            var task = RunAsynchronously();
            if (!task.IsCompleted)
                throw new InvalidOperationException("Only immediately returning steps can be run synchronously");
            task.GetAwaiter().GetResult();
        }

        private RunnableStep[] ProvideSteps()
        {
            var descriptors = _steps.ToArray();
            var totalStepsCount = descriptors.Length;
            string lastStepTypeName = null;
            var result = new RunnableStep[totalStepsCount];

            for (int i = 0; i < totalStepsCount; ++i)
            {
                var step = ToRunnableStep(descriptors[i], i, totalStepsCount, lastStepTypeName);
                result[i] = step;
                lastStepTypeName = step.Result.Info.Name.StepTypeName;
            }

            return result;
        }

        private RunnableStep ToRunnableStep(StepDescriptor descriptor, int stepIndex, int totalStepsCount, string lastStepTypeName)
        {
            var stepInfo = new StepInfo(_metadataProvider.GetStepName(descriptor, lastStepTypeName), stepIndex + 1, totalStepsCount);
            var parameters = descriptor.Parameters.Select(p => new StepParameter(p, _metadataProvider.GetStepParameterFormatter(p.ParameterInfo))).ToArray();
            return new RunnableStep(stepInfo, descriptor.StepInvocation, parameters, _exceptionToStatusMapper, _progressNotifier);
        }
    }
}