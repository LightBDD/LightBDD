using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using Moq;
using NUnit.Framework;
using System;
using LightBDD.Framework.Scenarios;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    public class Contextual_runner_tests
    {
        private class MyContext { }
        private Mock<ICoreScenarioStepsRunner> _stepsRunner;
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _stepsRunner = ScenarioMocks.CreateScenarioBuilder();
            _runner = ScenarioMocks.CreateBddRunner(_stepsRunner);
        }

        [Test]
        public void It_should_allow_to_apply_context_instance()
        {
            var capture = _stepsRunner.ExpectContext();
            var context = new object();

            _runner.WithContext(context);

            _stepsRunner.Verify();
            capture.Value.ContextResolver.Invoke(new FakeResolver())
                .ShouldBeSameAs(context);
        }

        [Test]
        public void It_should_allow_to_apply_context_provider()
        {
            var capture = _stepsRunner.ExpectContext();

            _runner.WithContext(() => TimeSpan.FromSeconds(5));

            _stepsRunner.Verify();
            capture.Value.ContextResolver.Invoke(new FakeResolver())
                .ShouldBe(TimeSpan.FromSeconds(5));
        }

        [Test]
        public void It_should_allow_to_apply_context_resolved_with_DI_container()
        {
            var resolver = new Mock<IDependencyResolver>();
            var capture = _stepsRunner.ExpectResolvedContext();

            _runner.WithContext<MyContext>();

            _stepsRunner.Verify();
            capture.Value.Invoke(resolver.Object);
            resolver.Verify(x => x.Resolve(typeof(MyContext)));
        }

        [Test]
        public void It_should_allow_configuring_context_on_instantiation()
        {
            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(r => r.Resolve<ConfigurableContext>())
                .Returns(new ConfigurableContext { Dependency = "dep1" });

            var capture = _stepsRunner.ExpectResolvedContext();

            _runner.WithContext<ConfigurableContext>(ctx => ctx.Text = "foo");

            var instance = (ConfigurableContext)capture.Value.Invoke(resolver.Object);
            Assert.AreEqual("foo", instance.Text);
            Assert.AreEqual("dep1", instance.Dependency);
        }

        [Test]
        public void It_should_allow_configuring_context_on_instantiation_when_using_resolver()
        {
            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(r => r.Resolve<ConfigurableContext>())
                .Returns(new ConfigurableContext { Dependency = "dep1" });

            var capture = _stepsRunner.ExpectResolvedContext();

            _runner.WithContext(
                r => r.Resolve<ConfigurableContext>(),
                ctx => ctx.Text = "foo");

            var instance = (ConfigurableContext)capture.Value.Invoke(resolver.Object);
            Assert.AreEqual("foo", instance.Text);
            Assert.AreEqual("dep1", instance.Dependency);
        }

        class ConfigurableContext
        {
            public string Text { get; set; }
            public string Dependency { get; set; }
        }

        class FakeResolver : IDependencyResolver
        {
            public object Resolve(Type type) => Activator.CreateInstance(type);
            public TDependency Resolve<TDependency>() => (TDependency)Resolve(typeof(TDependency));
        }
    }
}
