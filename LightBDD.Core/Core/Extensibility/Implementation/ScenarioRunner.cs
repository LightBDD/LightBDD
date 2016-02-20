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
        private StepDescriptor[] _steps = Arrays<StepDescriptor>.Empty();
        private INameInfo _name;
        private string[] _labels = Arrays<string>.Empty();
        private string[] _categories = Arrays<string>.Empty();

        public ScenarioRunner(ScenarioExecutor scenarioExecutor, IMetadataProvider metadataProvider, IProgressNotifier progressNotifier, Func<Exception, ExecutionStatus> exceptionToStatusMapper)
        {
            _scenarioExecutor = scenarioExecutor;
            _metadataProvider = metadataProvider;
            _progressNotifier = progressNotifier;
            _exceptionToStatusMapper = exceptionToStatusMapper;
        }

        public IScenarioRunner WithSteps(IEnumerable<StepDescriptor> steps)
        {
            _steps = steps.ToArray();
            return this;
        }

        private RunnableStep ToRunnableStep(StepDescriptor descriptor, int stepIndex)
        {
            var stepInfo = new StepInfo(_metadataProvider.GetStepName(descriptor), stepIndex + 1, _steps.Length);
            var parameters = descriptor.Parameters.Select(p => new StepParameter(p)).ToArray();
            return new RunnableStep(stepInfo, descriptor.StepInvocation, parameters, _exceptionToStatusMapper, _progressNotifier);
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
            if (name == null)
                throw new ArgumentNullException("name");
            _name = new NameInfo(name, Arrays<INameParameterInfo>.Empty());
            return this;
        }

        private IScenarioRunner WithName(INameInfo name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
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
                throw new ArgumentNullException("labels");
            _labels = labels;
            return this;
        }

        public IScenarioRunner WithCategories(string[] categories)
        {
            if (categories == null)
                throw new ArgumentNullException("categories");
            _categories = categories;
            return this;
        }

        public Task RunAsync()
        {
            Validate();
            return _scenarioExecutor.Execute(new ScenarioInfo(_name, _labels, _categories), _steps.Select(ToRunnableStep));
        }
    }
}