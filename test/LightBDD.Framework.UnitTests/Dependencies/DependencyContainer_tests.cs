using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Results;
using LightBDD.Framework.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Dependencies
{
    [TestFixture]
    public class DependencyContainer_tests
    {
        [Test]
        public async Task Steps_should_get_access_to_DI_container_via_GetDependencyResolver()
        {
            var resolvers = new List<IDependencyResolver>();

            void Step1()
            {
                resolvers.Add(StepExecution.Current.GetScenarioDependencyResolver());
            }

            void Step3()
            {
                resolvers.Add(StepExecution.Current.GetScenarioDependencyResolver());
            }

            void Step4()
            {
                resolvers.Add(StepExecution.Current.GetScenarioDependencyResolver());
            }

            TestCompositeStep Step2()
            {
                resolvers.Add(StepExecution.Current.GetScenarioDependencyResolver());
                return TestCompositeStep.Create(Step3, Step4);
            }

            await TestableBddRunner.Default.RunScenario(r => r.Test().TestScenario(TestStep.CreateSync(Step1), TestStep.CreateComposite(Step2)));

            Assert.That(resolvers.Count, Is.EqualTo(4));
            Assert.That(resolvers.Any(x => x == null), Is.False);
            Assert.That(resolvers.Distinct().Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Each_scenario_should_have_own_context()
        {
            var disposables = new List<Disposable>();

            void StepMethod()
            {
                var disposable = StepExecution.Current.GetScenarioDependencyResolver().Resolve<Disposable>();
                Assert.That(disposables.Count(x => !x.IsDisposed), Is.EqualTo(0));
                disposables.Add(disposable);
            }

            for (var i = 0; i < 4; ++i)
            {
                var scenario = await TestableBddRunner.Default.RunScenario(r => r.Test().TestScenario(StepMethod));
                scenario.Status.ShouldBe(ExecutionStatus.Passed);
            }

            Assert.That(disposables.Count(x => x.IsDisposed), Is.EqualTo(4));
        }

        class Disposable : IDisposable
        {
            public void Dispose()
            {
                IsDisposed = true;
            }

            public bool IsDisposed { get; private set; }
        }
    }
}