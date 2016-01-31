using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    static class TestSyntax
    {
        public static void TestScenario(this IBddRunner runner, params Action[] steps)
        {
            var coreRunner = runner.Integrate();
            var scenario = coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(s=>new StepDescriptor(s.Method.Name,StepInvocationHelper.FromAction(s))))
                .Build();
            coreRunner.RunScenarioAsync(scenario)
                .GetAwaiter()
                .GetResult();
        }
    }

    public static class StepInvocationHelper
    {
        public static Func<object, object[], Task> FromAction(Action invocation)
        {
            return (ctx, args) =>
            {
                invocation();
                return Task.FromResult(0);
            };
        } 
    }
}