using System;
using System.Collections.Generic;
using LightBDD.Core.Execution;

namespace LightBDD.Core.ExecutionContext.Implementation
{
    internal class CurrentStepProperty : IContextProperty
    {
        private readonly Stack<IStep> _steps = new Stack<IStep>();

        public IStep Step => _steps.Peek() ?? throw new InvalidOperationException("Current task is not executing any scenario steps. Ensure that feature is used within task running scenario step.");

        public void Stash(IStep step)
        {
            _steps.Push(step);
        }

        public void RemoveCurrent(IStep step)
        {
            var last = _steps.Pop();
            if (last != step)
                throw new InvalidOperationException("$Expected {step} to be current step but got {last}. Please report this issue on LightBDD project page.");
        }
    }
}