using System;
using LightBDD.Notification;

namespace LightBDD.Execution.Implementation
{
    internal class ExecutionContext
    {
        [ThreadStatic]
        private static ExecutionContext _instance;
        private IStep _currentStep;

        public ExecutionContext(IProgressNotifier progressNotifier, int totalStepCount)
        {
            ProgressNotifier = progressNotifier;
            TotalStepCount = totalStepCount;
        }

        public IProgressNotifier ProgressNotifier { get; private set; }
        public int TotalStepCount { get; private set; }

        public IStep CurrentStep
        {
            get
            {
                if (_currentStep == null)
                    throw new InvalidOperationException("Currently, no steps are being executed in this context.");
                return _currentStep;
            }
            set { _currentStep = value; }
        }

        public static ExecutionContext Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Current thread is not executing any scenarios. Please ensure that ExecutionContext is accessed from a thread running scenario.");
                return _instance;
            }
            set { _instance = value; }
        }
    }
}