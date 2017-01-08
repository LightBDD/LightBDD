using System;
using System.Collections.Generic;
using LightBDD.Core.Extensibility;

namespace LightBDD.Configuration
{
    //TODO: add tests
    public class ExecutionExtensionsConfiguration : IFeatureConfiguration
    {
        private readonly List<IScenarioExecutionExtension> _scenarioExtensions = new List<IScenarioExecutionExtension>();
        private readonly List<IStepExecutionExtension> _stepExtensions = new List<IStepExecutionExtension>();

        public IEnumerable<IScenarioExecutionExtension> ScenarioExecutionExtensions => _scenarioExtensions;
        public IEnumerable<IStepExecutionExtension> StepExecutionExtensions => _stepExtensions;

        public ExecutionExtensionsConfiguration AddScenarioExtension(IScenarioExecutionExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            _scenarioExtensions.Add(extension);
            return this;
        }

        public ExecutionExtensionsConfiguration AddStepExtension(IStepExecutionExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            _stepExtensions.Add(extension);
            return this;
        }
    }
}