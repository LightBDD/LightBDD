using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.Framework.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class CurrentScenarioProperty : IContextProperty
    {
        private IScenario _scenario;

        public IScenario Scenario
        {
            get
            {
                var scenario = _scenario;
                if (scenario != null)
                    return scenario;
                throw new InvalidOperationException($"Current task is not executing any scenarios or current step tracking feature is not enabled in {nameof(LightBddConfiguration)}. Ensure that configuration.{nameof(ConfigurationExtensions.ExecutionExtensionsConfiguration)}().{nameof(FrameworkConfigurationExtensions.EnableCurrentScenarioTracking)}() is called during LightBDD initialization and feature is used within task running scenario.");
            }
            set => _scenario = value;
        }
    }
}