using System;
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
                    return _step;
                throw new InvalidOperationException($"No scenario step is being executed with this task, or {nameof(StepCommentingExtension)} is not installed.");
            }
            set { _step = value; }
        }
    }
}