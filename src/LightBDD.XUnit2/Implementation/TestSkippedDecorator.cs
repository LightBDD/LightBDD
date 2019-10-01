using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.XUnit2.Implementation.Customization;

namespace LightBDD.XUnit2.Implementation
{
    internal class TestSkippedDecorator : IScenarioDecorator
    {
        public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            var skipReason = TestContextProvider.Current.SkipReason;
            if (!string.IsNullOrWhiteSpace(skipReason))
                throw new IgnoreException(skipReason);

            return scenarioInvocation.Invoke();
        }
    }
}