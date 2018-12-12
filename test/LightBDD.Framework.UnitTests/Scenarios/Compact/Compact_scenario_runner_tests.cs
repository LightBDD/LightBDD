using LightBDD.Framework.Scenarios.Compact;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        class MyContext
        {
            public readonly List<string> Executed = new List<string>();
        }
    }
}
