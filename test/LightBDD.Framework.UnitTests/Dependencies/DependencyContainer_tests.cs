using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Commenting;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.ExecutionContext.Configuration;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Dependencies
{
    [TestFixture]
    public class DependencyContainer_tests
    {
        [Test]
        public void Steps_should_get_access_to_DI_container_via_GetDependencyResolver()
        {
            IDependencyResolver step1Resolver = null,
                step2Resolver = null,
                step3Resolver = null,
                step4Resolver = null;

            void Step1()
            {
                step1Resolver = StepExecution.Current.GetDependencyResolver();
            }

            void Step3()
            {
                step3Resolver = StepExecution.Current.GetDependencyResolver();
            }

            void Step4()
            {
                step4Resolver = StepExecution.Current.GetDependencyResolver();
            }

            TestCompositeStep Step2()
            {
                step2Resolver = StepExecution.Current.GetDependencyResolver();
                return TestCompositeStep.Create(Step3, Step4);
            }

            GetFeatureRunner().GetBddRunner(this).Test().TestScenario(TestStep.CreateSync(Step1), TestStep.CreateComposite(Step2));

            Assert.That(step1Resolver, Is.Not.Null);
            Assert.That(step2Resolver, Is.Not.Null);
            Assert.That(step3Resolver, Is.Not.Null);
            Assert.That(step4Resolver, Is.Not.Null);
            Assert.That(new[] { step1Resolver, step2Resolver, step3Resolver, step4Resolver }.Distinct().Count(), Is.EqualTo(4));
        }

        [Test]
        public void Each_step_should_have_own_context()
        {
            var disposables = new List<Disposable>();

            void StepMethod()
            {
                var disposable = StepExecution.Current.GetDependencyResolver().Resolve<Disposable>();
                Assert.That(disposables.Count(x => !x.IsDisposed), Is.EqualTo(0));
                disposables.Add(disposable);
            }

            Assert.DoesNotThrow(() => GetFeatureRunner().GetBddRunner(this).Test().TestScenario(StepMethod, StepMethod, StepMethod, StepMethod));

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

        private IFeatureRunner GetFeatureRunner()
        {
            var context = TestableIntegrationContextBuilder.Default()
                .WithConfiguration(cfg => cfg.ExecutionExtensionsConfiguration().EnableCurrentStepManagement());

            return new TestableFeatureRunnerRepository(context).GetRunnerFor(GetType());
        }
    }
}