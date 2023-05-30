using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests;

[TestFixture]
public class CoreBddRunner_dependencies_injection_tests
{
    private TestableDependencyContainer _container;

    class Dependency { }

    class SomeFeatureFixture
    {
        [FixtureDependency]
        public Dependency Dependency { get; set; }
        public Dependency Other { get; set; }

        public bool WasDependencySet { get; private set; }
        public bool WasOtherSet { get; private set; }

        public void CheckDependenciesSet()
        {
            WasDependencySet = Dependency != null;
            WasOtherSet = Other != null;
        }
    }

    class InvalidDependencies
    {
        [FixtureDependency]
        public Dependency PrivateSetter { get; }

        [FixtureDependency]
        public static Dependency Static { get; set; }

        public void Foo() { }
    }

    class InvalidDerived : InvalidDependencies
    {
        [FixtureDependency]
        public Dependency OtherPrivateSetter { get; }
    }

    class FixtureWithSetUp : IScenarioSetUp
    {
        [FixtureDependency]
        public Dependency Dependency { get; set; }

        public bool DependencySetOnSetUp { get; private set; }

        public Task OnScenarioSetUp()
        {
            DependencySetOnSetUp = Dependency != null;
            return Task.CompletedTask;
        }
        public void Foo() { }
    }

    [SetUp]
    public void SetUp()
    {
        _container = new TestableDependencyContainer();
    }

    private TestableFeatureRunnerRepository SetupRepository(Action<LightBddConfiguration> cfg = null)
    {
        return new TestableFeatureRunnerRepository(TestableIntegrationContextBuilder.Default()
            .WithConfiguration(c =>
            {
                c.DependencyContainerConfiguration().UseContainer(_container);
                cfg?.Invoke(c);
            }));
    }

    [Test]
    public void Runner_should_inject_dependencies_for_properties_with_FixtureDependency_attribute()
    {
        _container.Register(typeof(Dependency), new Dependency());
        var fixture = new SomeFeatureFixture();

        var runner = SetupRepository().GetRunnerFor(typeof(SomeFeatureFixture)).GetBddRunner(fixture);
        runner.Test().TestScenario(fixture.CheckDependenciesSet);

        Assert.That(fixture.WasDependencySet, Is.True);
        Assert.That(fixture.WasOtherSet, Is.False);
    }

    [Test]
    public void Runner_should_fail_if_FixtureDependency_is_incorrectly_used()
    {
        var fixture = new InvalidDerived();
        var runner = SetupRepository().GetRunnerFor(typeof(InvalidDerived)).GetBddRunner(fixture);

        var ex = Assert.Throws<InvalidOperationException>(() => runner.Test().TestScenario(fixture.Foo));
        Assert.That(ex.Message, Does.Contain("Unable to inject dependencies on 'InvalidDerived':"));
        Assert.That(ex.Message, Does.Contain("'InvalidDependencies.PrivateSetter' property has to have a public setter"));
        Assert.That(ex.Message, Does.Contain("'InvalidDerived.OtherPrivateSetter' property has to have a public setter"));
        Assert.That(ex.Message, Does.Contain("'InvalidDependencies.Static' property cannot be static"));
    }

    [Test]
    public void Runner_should_fail_if_FixtureDependency_fails_resolution()
    {
        var fixture = new SomeFeatureFixture();
        var runner = SetupRepository().GetRunnerFor(typeof(SomeFeatureFixture)).GetBddRunner(fixture);

        var ex = Assert.Throws<InvalidOperationException>(() => runner.Test().TestScenario(fixture.CheckDependenciesSet));
        Assert.That(ex.Message, Does.Contain($"Unable to inject dependency of type '{nameof(Dependency)}' to 'SomeFeatureFixture.Dependency' property: "));
    }

    [Test]
    public void Runner_should_call_decorators_on_already_configured_fixtures()
    {
        var dependency = new Dependency();
        _container.Register(typeof(Dependency), dependency);
        var decorator = new CapturingDecorator();
        var fixture = new SomeFeatureFixture();

        SetupRepository(cfg => cfg.ExecutionExtensionsConfiguration().EnableScenarioDecorator(() => decorator))
            .GetRunnerFor(typeof(SomeFeatureFixture))
            .GetBddRunner(fixture)
            .Test()
            .TestScenario(fixture.CheckDependenciesSet);

        Assert.That(decorator.Captured, Is.SameAs(dependency));
    }

    [Test]
    public void Runner_should_call_OnScenarioSetUp_on_already_configured_fixtures()
    {
        var dependency = new Dependency();
        _container.Register(typeof(Dependency), dependency);
        var fixture = new FixtureWithSetUp();

        SetupRepository()
            .GetRunnerFor(typeof(FixtureWithSetUp))
            .GetBddRunner(fixture)
            .Test()
            .TestScenario(fixture.Foo);

        Assert.That(fixture.DependencySetOnSetUp, Is.True);
    }

    class CapturingDecorator : IScenarioDecorator
    {
        public Dependency Captured { private set; get; }

        public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            Captured = (scenario.Fixture as SomeFeatureFixture)?.Dependency;
            return Task.CompletedTask;
        }
    }
}