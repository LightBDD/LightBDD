using System;
using System.Globalization;
using System.Threading;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioContext
    {
        private static readonly AsyncLocal<ScenarioContext> CurrentContext = new AsyncLocal<ScenarioContext>();
        private RunnableStep _currentStep;
        public Exception ScenarioInitializationException { get; private set; }
        public RunnableStep[] PreparedSteps { get; private set; } = Array.Empty<RunnableStep>();
        public object ExecutionContext { get; private set; }

        public void InitializeScenario(Func<RunnableStep[]> stepsProvider, Func<object> contextProvider)
        {
            try
            {
                PreparedSteps = PrepareSteps(stepsProvider);
                ExecutionContext = CreateExecutionContext(contextProvider);
            }
            catch (Exception e)
            {
                ScenarioInitializationException = e;
                throw;
            }
        }

        private static RunnableStep[] PrepareSteps(Func<RunnableStep[]> stepsProvider)
        {
            try
            {
                return stepsProvider.Invoke();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Step initialization failed: {e.Message}", e);
            }
        }

        private static object CreateExecutionContext(Func<object> contextProvider)
        {
            try
            {
                return contextProvider.Invoke();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Context initialization failed: {e.Message}", e);
            }
        }

        public static ScenarioContext Current
        {
            get
            {
                var value = CurrentContext.Value;
                if (value == null)
                    throw new InvalidOperationException("Current task is not executing any scenarios. Please ensure that ScenarioContext is accessed from task running scenario.");
                return value;
            }
            set { CurrentContext.Value = value; }
        }

        public static CultureInfo GetCurrentCulture() => CurrentContext.Value?.Culture ?? CultureInfo.DefaultThreadCurrentCulture;

        public RunnableStep CurrentStep
        {
            get
            {
                if (_currentStep == null)
                    throw new InvalidOperationException("Currently, no steps are being executed in this context.");
                return _currentStep;
            }
            set { _currentStep = value; }
        }

        public CultureInfo Culture { get; } = CultureInfo.CurrentCulture;
    }
}