using System;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioData
    {
        public Exception ScenarioInitializationException { get; private set; }
        public RunnableStep[] PreparedSteps { get; private set; } = new RunnableStep[0];
        public object ScenarioContext { get; private set; }

        public void InitializeScenario(Func<RunnableStep[]> stepsProvider, Func<object> contextProvider)
        {
            try
            {
                PreparedSteps = PrepareSteps(stepsProvider);
                ScenarioContext = CreateExecutionContext(contextProvider);
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
    }
}