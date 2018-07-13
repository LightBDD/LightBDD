using System;
using LightBDD.Framework.Scenarios.Basic;
using Xunit;
using Xunit.Abstractions;
#pragma warning disable xUnit1026

namespace LightBDD.XUnit2.UnitTests
{
    public class IntegrationTests_with_explicit_ITestOutputHelper : FeatureFixture
    {
        public IntegrationTests_with_explicit_ITestOutputHelper(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void TestOutput_should_be_initialized_when_used_with_fact()
        {
            Assert.NotNull(TestOutput);
        }

        [Theory]
        [InlineData(true)]
        public void TestOutput_should_be_initialized_when_used_with_theory(bool value)
        {
            Assert.NotNull(TestOutput);
        }

        [Fact]
        public void Runner_should_throw_meaningful_exception_if_scenario_is_not_run_from_Scenario_attribute()
        {
            Exception ex = Assert.Throws<InvalidOperationException>(() => Runner.RunScenario(Some_step));
            Assert.Equal(
                "Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute.",
                ex.Message);
        }

        private void Some_step()
        {
        }
    }
}