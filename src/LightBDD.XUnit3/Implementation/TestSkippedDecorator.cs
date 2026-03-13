using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.XUnit3.Implementation
{
    internal class TestSkippedDecorator : IScenarioDecorator
    {
        public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            var skipReason = Customization.SkipReasonProvider.Current;
            if (!string.IsNullOrWhiteSpace(skipReason))
                throw new Customization.IgnoreException(skipReason);

            return scenarioInvocation.Invoke();
        }
    }
}
