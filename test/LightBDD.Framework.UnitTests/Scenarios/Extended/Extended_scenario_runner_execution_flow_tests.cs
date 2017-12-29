using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended
{
    [TestFixture]
    public class Extended_scenario_runner_execution_flow_tests : ExtendedScenariosTestBase<NoContext>
    {
        [Test]
        public void It_should_allow_to_run_synchronous_scenarios()
        {
            ExpectSynchronousScenarioRun();

            Runner.RunScenario(
                _ => Step_one(),
                _ => Step_two());

            MockScenarioRunner.Verify();
        }

        [Test]
        public async Task It_should_allow_to_run_asynchronous_scenarios()
        {
            ExpectAsynchronousScenarioRun();

            await Runner.RunScenarioAsync(
                _ => Step_one_async(),
                _ => Step_two_async());

            MockScenarioRunner.Verify();
        }
    }
}
