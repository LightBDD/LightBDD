using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;
using Ploeh.AutoFixture;

namespace LightBDD.UnitTests.Helpers
{
    public static class MockFixture
    {
        public static Fixture CreateNew()
        {
            var autoFixture = new Fixture();
            autoFixture.Register(() => new ExecutionTime(DateTimeOffset.Now, TimeSpan.FromMilliseconds(2634723)));
            autoFixture.Register<IFeatureInfo>(() => autoFixture.Create<Mocks.TestFeatureInfo>());
            autoFixture.Register<IFeatureResult>(() => autoFixture.Create<Mocks.TestFeatureResult>());
            autoFixture.Register<IScenarioInfo>(() => autoFixture.Create<Mocks.TestScenarioInfo>());
            autoFixture.Register<IScenarioResult>(() => autoFixture.Create<Mocks.TestScenarioResult>());
            autoFixture.Register<IStepInfo>(() => autoFixture.Create<Mocks.TestStepInfo>());
            autoFixture.Register<IStepResult>(() => autoFixture.Create<Mocks.TestStepResult>());
            autoFixture.Register<IStepNameInfo>(() => autoFixture.Create<Mocks.TestStepNameInfo>());
            autoFixture.Register<INameInfo>(() => autoFixture.Create<Mocks.TestNameInfo>());
            autoFixture.Register<INameParameterInfo>(() => autoFixture.Create<Mocks.TestNameParameterInfo>());
            return autoFixture;
        }
    }
}