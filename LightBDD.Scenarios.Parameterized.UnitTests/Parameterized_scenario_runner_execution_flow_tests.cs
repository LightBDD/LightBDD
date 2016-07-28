using System.Threading.Tasks;
using LightBDD.Scenarios.Parameterized.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Scenarios.Parameterized.UnitTests
{
    [TestFixture]
    public class Parameterized_scenario_runner_execution_flow_tests : ParameterizedScenrariosTestBase<NoContext>
    {
        [Test]
        public void It_should_allow_to_run_synchronous_scenarios()
        {
            ExpectSynchronousScenarioRun();

            Runner.Parameterized().RunScenario(
                _ => Step_one(),
                _ => Step_two());

            Runner.VerifyAllExpectations();
            MockScenarioRunner.VerifyAllExpectations();
        }

        [Test]
        public async Task It_should_allow_to_run_asynchronous_scenarios()
        {
            ExpectAsynchronousScenarioRun();

            await Runner.Parameterized().RunScenarioAsync(
                _ => Step_one_async(),
                _ => Step_two_async());

            Runner.VerifyAllExpectations();
            MockScenarioRunner.VerifyAllExpectations();
        }
    }
}
