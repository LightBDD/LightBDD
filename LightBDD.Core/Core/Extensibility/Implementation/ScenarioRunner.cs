using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Helpers;
using LightBDD.Core.Implementation;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class ScenarioRunner : IScenarioRunner
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        private readonly ScenarioExecutor _scenarioExecutor;
        private StepDescriptor[] _steps = Arrays<StepDescriptor>.Empty();
        private INameInfo _name;
        private string[] _labels = Arrays<string>.Empty();
        private string[] _categories = Arrays<string>.Empty();

        public ScenarioRunner(IMetadataProvider metadataProvider, Func<Exception, ExecutionStatus> exceptionToStatusMapper, ScenarioExecutor scenarioExecutor)
        {
            _metadataProvider = metadataProvider;
            _exceptionToStatusMapper = exceptionToStatusMapper;
            _scenarioExecutor = scenarioExecutor;
        }

        public IScenarioRunner WithSteps(IEnumerable<StepDescriptor> steps)
        {
            _steps = steps.ToArray();
            return this;
        }

        private RunnableStep ToRunnableStep(StepDescriptor descriptor, int stepIndex)
        {
            var stepInfo = new StepInfo(_metadataProvider.GetStepName(descriptor), stepIndex + 1);
            var parameters = descriptor.Parameters.Select(p => new StepParameter(p)).ToArray();
            return new RunnableStep(stepInfo, descriptor.StepInvocation, parameters, _exceptionToStatusMapper);
        }

        public IScenarioRunner WithCapturedScenarioDetails()
        {
            var methodInfo = _metadataProvider.CaptureCurrentScenarioMethod();

            return WithName(_metadataProvider.GetScenarioName(methodInfo))
                .WithLabels(_metadataProvider.GetScenarioLabels(methodInfo))
                .WithCategories(_metadataProvider.GetScenarioCategories(methodInfo));
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