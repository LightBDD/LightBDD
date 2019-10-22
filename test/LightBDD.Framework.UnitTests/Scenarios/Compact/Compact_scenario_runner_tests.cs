using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;

namespace LightBDD.Framework.UnitTests.Scenarios.Compact
{
    [TestFixture]
    public class Compact_scenario_runner_tests
    {
        [Test]
        public async Task It_should_allow_to_add_steps_to_scenario()
        {
            var builder = new TestableScenarioBuilder<MyContext>();

            var stepOne = "Step one";
            var stepTwo = "Step two";
            var stepThree = "Step three";
            var expected = new[] { stepOne, stepTwo, stepThree };

            builder.AddStep(stepOne, ctx => ctx.Executed.Add(stepOne));
            builder.AddStep(stepTwo, ctx => ctx.Executed.Add(stepTwo));
            builder.AddAsyncStep(stepThree, async ctx =>
            {
                await Task.Yield();
                ctx.Executed.Add(stepThree);
            });

            var steps = builder.Steps;
            Assert.That(steps.Select(x => x.RawName).ToArray(), Is.EqualTo(expected));

            var context = new MyContext();
            foreach (var step in steps)
                await step.StepInvocation.Invoke(context, null);

            Assert.That(context.Executed, Is.EqualTo(expected));
        }

        [Test]
        public async Task It_should_allow_to_add_steps_to_composite()
        {
            var stepOne = "Step one";
            var stepTwo = "Step two";
            var stepThree = "Step three";
            var expected = new[] { stepOne, stepTwo, stepThree };

            var context = new MyContext();
            var composite = CompositeStep.DefineNew()
                .WithContext(context)
                .AddStep(stepOne, ctx => ctx.Executed.Add(stepOne))
                .AddStep(stepTwo, ctx => ctx.Executed.Add(stepTwo))
                .AddAsyncStep(stepThree, async ctx =>
                {
                    await Task.Yield();
                    ctx.Executed.Add(stepThree);
                })
                .Build();

            var steps = composite.SubSteps.ToArray();
            Assert.That(steps.Select(x => x.RawName).ToArray(), Is.EqualTo(expected));

            foreach (var step in steps)
                await step.StepInvocation.Invoke(context, null);

            Assert.That(context.Executed, Is.EqualTo(expected));
        }

        [Test]
        public async Task It_should_parse_composite_names()
        {
            var featureRunner = TestableFeatureRunnerRepository.GetRunner(GetType());
            var runner = featureRunner.GetBddRunner(this);

            var expectedValue = "Some\r\nmultiline\tresponse";

            await runner
                .AddStep("Given my scenario with value [55]", _ => { })
                .AddStep(" When I send My_Request<int> for it ", _ => { })
                .AddAsyncStep($"Only then I should receive \"{expectedValue}\"", _ => Task.FromResult(0))
                .RunAsync();

            var steps = featureRunner.GetFeatureResult().GetScenarios().Single().GetSteps();

            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "GIVEN my scenario with value [55]", ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, "WHEN I send My_Request<int> for it", ExecutionStatus.Passed),
                new StepResultExpectation(3, 3, "Only then I should receive \"Some multiline response\"", ExecutionStatus.Passed));
        }

        class MyContext
        {
            public readonly List<string> Executed = new List<string>();
        }
    }
}
