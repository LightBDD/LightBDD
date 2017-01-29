using System;
using LightBDD.Commenting.Configuration;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.ExecutionContext;

namespace LightBDD.Commenting.Implementation
{
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
                throw new InvalidOperationException($"Current task is not executing any scenario steps or commenting feature is not enabled in {nameof(LightBddConfiguration)}. Ensure that configuration.{nameof(ConfigurationExtensions.ExecutionExtensionsConfiguration)}().{nameof(CommentingConfigurationExtensions.EnableStepCommenting)}() is called during LightBDD initialization and commenting feature is called from task running scenario step.");
            }
            set { _step = value; }
        }
    }
}