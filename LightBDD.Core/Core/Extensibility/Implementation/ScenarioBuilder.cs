using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Helpers;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class ScenarioBuilder : IScenarioBuilder
    {
        private readonly IMetadataProvider _metadataProvider;
        private IStepInfo[] _steps = Arrays<IStepInfo>.Empty();
        private INameInfo _name;
        private string[] _labels = Arrays<string>.Empty();
        private string[] _categories = Arrays<string>.Empty();

        public ScenarioBuilder(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IScenarioBuilder WithSteps(IEnumerable<StepDescriptor> steps)
        {
            _steps=steps.Select(s => (IStepInfo)new StepInfo(s.StepInvocation, _metadataProvider.GetStepName(s))).ToArray();
            return this;
        }

        public IScenarioBuilder WithCapturedScenarioDetails()
        {
            var methodInfo = _metadataProvider.CaptureCurrentScenarioMethod();

            return WithName(_metadataProvider.GetScenarioName(methodInfo))
                .WithLabels(_metadataProvider.GetScenarioLabels(methodInfo))
                .WithCategories(_metadataProvider.GetScenarioCategories(methodInfo));
        }

        private IScenarioBuilder WithName(INameInfo name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            _name = name;
            return this;
        }

        public IScenarioInfo Build()
        {
            if (_name == null)
                throw new ArgumentNullException("Name", "Scenario name is not provided.");
            return new ScenarioInfo(_name, _steps, _labels, _categories);
        }

        public IScenarioBuilder WithLabels(string[] labels)
        {
            if (labels == null)
                throw new ArgumentNullException("labels");
            _labels = labels;
            return this;
        }

        public IScenarioBuilder WithCategories(string[] categories)
        {
            if (categories == null)
                throw new ArgumentNullException("categories");
            _categories = categories;
            return this;
        }
    }
}