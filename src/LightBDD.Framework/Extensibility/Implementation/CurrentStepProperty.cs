using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.Framework.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class CurrentStepProperty : IContextProperty
    {
        private IStep _step;

        public IStep Step
        {
            get
            {
                var step = _step;
                if (step != null)
                    return step;
                throw new InvalidOperationException($"Current task is not executing any scenario steps or current step management feature is not enabled in {nameof(LightBddConfiguration)}. Ensure that configuration.{nameof(ConfigurationExtensions.ExecutionExtensionsConfiguration)}().{nameof(FrameworkConfigurationExtensions.EnableCurrentScenarioTracking)}() is called during LightBDD initialization and feature is used within task running scenario step.");
            }
        }

        public IStep Update(IStep newStep)
        {
            var old = _step;
            _step = newStep;
            return old;
        }
    }
}