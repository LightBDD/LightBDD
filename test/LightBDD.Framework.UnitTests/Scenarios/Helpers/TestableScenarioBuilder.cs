using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.UnitTests.Scenarios.Helpers
{
    public class TestableScenarioBuilder<T> :  IIntegratedScenarioBuilder<T>
    {
        public readonly List<StepDescriptor> Steps = new List<StepDescriptor>();


        public IIntegratedScenarioBuilder<T> Integrate()
        {
            throw new NotImplementedException();
        }

        public async Task RunAsync()
        {
            throw new NotImplementedException();
        }

        public ICoreScenarioBuilder Core { get; }
        public IIntegratedScenarioBuilder<T> Configure(Action<ICoreScenarioBuilder> builder)
        {
            throw new NotImplementedException();
        }

        public Func<Task> Build()
        {
            throw new NotImplementedException();
        }
    }
}