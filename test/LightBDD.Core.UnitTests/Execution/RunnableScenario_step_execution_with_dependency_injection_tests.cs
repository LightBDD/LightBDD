using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class RunnableScenario_step_execution_with_dependency_injection_tests
    {

        [Test]
        public async Task Runner_should_instantiate_scenario_context_within_scenario_scope()
        {
            MyScenarioContext captured = null;

            var result = await TestableScenarioFactory.Default.RunScenario(x => x.Test()
                .WithContext(r => captured = r.Resolve<MyScenarioContext>().VerifyNotDisposed())
                .TestScenario(Scenario_step));
            result.Status.ShouldBe(ExecutionStatus.Passed);

            captured.ShouldNotBeNull();
            captured.Disposed.ShouldBeTrue();
        }

        [Test]
        public async Task Runner_should_instantiate_step_context_within_scenario_scope()
        {
            MyScenarioContext captured = null;
            var result = await TestableScenarioFactory.Default.RunScenario(x => x.Test()
                .WithContext(r => captured = r.Resolve<MyScenarioContext>().VerifyNotDisposed())
                .TestGroupScenario(Given_composite, Given_composite));
            result.Status.ShouldBe(ExecutionStatus.Passed);

            captured.ShouldNotBeNull();
            captured.Disposed.ShouldBeTrue();
            captured.StepContexts.Distinct().Count().ShouldBe(2);
            captured.StepContexts.ShouldAllBe(s => s.Disposed);
        }

        class MyScenarioContext : IAsyncDisposable
        {
            public readonly List<MyStepContext> StepContexts = new();

            public MyScenarioContext VerifyNotDisposed()
            {
                if (Disposed)
                    throw new Exception("Already disposed");
                return this;
            }

            public ValueTask DisposeAsync()
            {
                Disposed = true;
                return default;
            }

            public bool Disposed { get; private set; }
            
            public MyStepContext CaptureStepContext(MyStepContext step)
            {
                StepContexts.Add(step);
                return step;
            }
        }

        class MyStepContext : IAsyncDisposable
        {
            public MyStepContext VerifyNotDisposed()
            {
                if (Disposed)
                    throw new Exception("Already disposed");
                return this;
            }

            public ValueTask DisposeAsync()
            {
                Disposed = true;
                return default;
            }

            public bool Disposed { get; private set; }
        }

        private void Step_step()
        {
            ((MyStepContext)ScenarioExecutionContext.CurrentStep.Context).VerifyNotDisposed();
        }

        private void Scenario_step()
        {
            ((MyScenarioContext)ScenarioExecutionContext.CurrentStep.Context).VerifyNotDisposed();
        }

        private TestCompositeStep Given_composite()
        {
            var scenarioContext = (MyScenarioContext)ScenarioExecutionContext.CurrentStep.Context;
            return new TestCompositeStep(
                Resolvable.Use<object>(r => scenarioContext.CaptureStepContext(r.Resolve<MyStepContext>().VerifyNotDisposed())),
                TestStep.CreateSync(Step_step));
        }
    }
}