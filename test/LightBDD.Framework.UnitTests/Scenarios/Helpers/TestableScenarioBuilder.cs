using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightBDD.Framework.UnitTests.Scenarios.Helpers
{
    public class TestableScenarioBuilder<T> : IIntegratedScenarioBuilder<T>
    {
        public readonly List<StepDescriptor> Steps;

        public TestableScenarioBuilder()
        {
            var builder = ScenarioMocks.CreateScenarioBuilder();
            //TODO: review
            builder.SetupConfiguration(new LightBddConfiguration());
            Steps = builder.ExpectAddSteps();
            Core = builder.Object;
        }

        public IIntegratedScenarioBuilder<T> Integrate() => this;

        public Task RunAsync()
        {
            throw new NotImplementedException();
        }

        public ICoreScenarioStepsRunner Core { get; }

    }
}