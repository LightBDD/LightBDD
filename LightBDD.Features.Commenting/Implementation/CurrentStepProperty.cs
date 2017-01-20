using System;
using LightBDD.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Extensions.ContextualAsyncExecution;

namespace LightBDD.Implementation
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
                throw new InvalidOperationException($"Current task is not executing any scenario steps or commenting feature is not enabled in {nameof(LightBddConfiguration)}. Ensure that cfg.{nameof(ConfigurationExtensions.ExecutionExtensionsConfiguration)}().{nameof(StepCommentingConfiguration.EnableStepCommenting)}() is called during LightBDD initialization and commenting feature is called from task running scenario step.");
            }
            set { _step = value; }
        }
    }
}