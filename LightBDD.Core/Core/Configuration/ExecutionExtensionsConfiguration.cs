using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Core.Configuration
{
    public class ExecutionExtensionsConfiguration : IExecutionExtensions, IFeatureConfiguration
    {
        private readonly List<IScenarioExecutionExtension> _scenarioExtensions = new List<IScenarioExecutionExtension>();
        private readonly List<IStepExecutionExtension> _stepExtensions = new List<IStepExecutionExtension>();

        public IEnumerable<IScenarioExecutionExtension> ScenarioExecutionExtensions => _scenarioExtensions;
        public IEnumerable<IStepExecutionExtension> StepExecutionExtensions => _stepExtensions;

        public ExecutionExtensionsConfiguration EnableScenarioExtension<TScenarioExecutionExtension>()
            where TScenarioExecutionExtension : IScenarioExecutionExtension, new()
            => EnableScenarioExtension(() => new TScenarioExecutionExtension());

        public ExecutionExtensionsConfiguration EnableScenarioExtension<TScenarioExecutionExtension>(Func<TScenarioExecutionExtension> factory) where TScenarioExecutionExtension : IScenarioExecutionExtension
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!_scenarioExtensions.Any(e => e is TScenarioExecutionExtension))
                _scenarioExtensions.Add(factory());

            return this;
        }

        public ExecutionExtensionsConfiguration EnableStepExtension<TStepExecutionExtension>(Func<TStepExecutionExtension> factory) where TStepExecutionExtension : IStepExecutionExtension
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!_stepExtensions.Any(e => e is TStepExecutionExtension))
                _stepExtensions.Add(factory());

            return this;
        }

        public ExecutionExtensionsConfiguration EnableStepExtension<TStepExecutionExtension>()
           where TStepExecutionExtension : IStepExecutionExtension, new()
           => EnableStepExtension(() => new TStepExecutionExtension());
    }
}