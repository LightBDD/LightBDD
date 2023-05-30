using System;
using LightBDD.Core.Configuration;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests;

[TestFixture]
public class CoreBddRunner_dependencies_injection_tests
{
    private TestableDependencyContainer _container;
    private TestableFeatureRunnerRepository _runners;

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

    [SetUp]
    public void SetUp()
    {
        _container = new TestableDependencyContainer();
        _runners = new TestableFeatureRunnerRepository(TestableIntegrationContextBuilder.Default()
            .WithConfiguration(c => c.DependencyContainerConfiguration().UseContainer(_container)));

    }

    [Test]
    public void Runner_should_inject_dependencies_for_properties_with_FixtureDependency_attribute()
    {
        _container.Register(typeof(Dependency), new Dependency());
        var fixture = new SomeFeatureFixture();

        var runner = _runners.GetRunnerFor(typeof(SomeFeatureFixture)).GetBddRunner(fixture);
        runner.Test().TestScenario(fixture.CheckDependenciesSet);

        Assert.That(fixture.WasDependencySet, Is.True);
        Assert.That(fixture.WasOtherSet, Is.False);
    }

    [Test]
    public void Runner_should_fail_if_FixtureDependency_is_incorrectly_used()
    {
        var fixture = new InvalidDerived();
        var runner = _runners.GetRunnerFor(typeof(InvalidDerived)).GetBddRunner(fixture);

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
        var runner = _runners.GetRunnerFor(typeof(SomeFeatureFixture)).GetBddRunner(fixture);

        var ex = Assert.Throws<InvalidOperationException>(() => runner.Test().TestScenario(fixture.CheckDependenciesSet));
        Assert.That(ex.Message, Does.Contain($"Unable to inject dependency of type '{nameof(Dependency)}' to 'SomeFeatureFixture.Dependency' property: "));
    }
}