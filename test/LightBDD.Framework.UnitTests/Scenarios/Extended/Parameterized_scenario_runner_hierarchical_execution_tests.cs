using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended
{
    [TestFixture]
    public class Parameterized_scenario_runner_hierarchical_execution_tests : ParameterizedScenariosTestBase<NoContext>
    {
        [Test]
        public void It_should_run_grouped_tests()
        {
            ExpectSynchronousScenarioRun();

            Runner.RunScenario(_ => Step_group());

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(1));

            Assert.DoesNotThrow(() => CapturedSteps[0].StepInvocation.Invoke(null, null));
            Assert.Ignore("not completed");
        }

        private StepGroup Step_group()
        {
            return Runner.DefineStepGroup(
                _ => Given_step(),
                _ => When_step(),
                _ => Then_step());
        }

        private void Then_step()
        {
            throw new NotImplementedException();
        }

        private void When_step()
        {
            throw new NotImplementedException();
        }

        private void Given_step()
        {
            throw new NotImplementedException();
        }

        private async void Step_one_async_void()
        {
            await Task.Delay(200);
        }
    }
}