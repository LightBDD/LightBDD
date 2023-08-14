#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class RunnableScenario_fixture_setup_and_teardown_tests
{
    private readonly Fixture _fixture = new();
    private readonly TestableScenarioFactory _factory;

    public RunnableScenario_fixture_setup_and_teardown_tests()
    {
        _factory = TestableScenarioFactory.Create(cfg => cfg.RegisterFixtureFactory(x => x.Use(new FakeFixtureFactory(_fixture))));
    }

    [Test]
    public async Task Runner_should_call_OnScenarioSetUp_and_OnScenarioTearDown_when_provided()
    {
        await _factory.RunScenario<Fixture>((fixture, runner) => runner.Test().TestScenario(fixture.Pass));

        Assert.NotNull(_fixture);
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
    }

    [Test]
    public async Task Runner_should_call_OnScenarioTearDown_on_step_failure()
    {
        await _factory.RunScenario<Fixture>((fixture, runner) => runner.Test().TestScenario(fixture.Fail));

        Assert.NotNull(_fixture);
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
    }

    [Test]
    public async Task Runner_should_call_OnScenarioTearDown_on_SetUp_failure_and_propagate_SetUp_exception_without_calling_steps()
    {
        _fixture.ThrowOnSetUp = true;
        var result = await _factory.RunScenario<Fixture>((fixture, runner) => runner.Test().TestScenario(fixture.Pass));

        Assert.NotNull(_fixture);
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
        Assert.That(_fixture.PassCalled, Is.False);
        result.ExecutionException.ShouldBeOfType<InvalidOperationException>()
            .Message.ShouldBe("OnScenarioSetUp() failed: OnSetUp");
    }

    [Test]
    public async Task Runner_should_propagate_steps_and_TearDown_exceptions()
    {
        _fixture.ThrowOnTearDown = true;
        var result = await _factory.RunScenario<Fixture>((fixture, runner) => runner.Test().TestScenario(fixture.Fail));

        Assert.NotNull(_fixture);
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);

        result.ExecutionException.ShouldBeOfType<AggregateException>()
            .InnerExceptions.Select(e => e.Message).ShouldBe(new[] { "Fixture OnScenarioTearDown() failed: OnTearDown", "IO" });
    }

    class Fixture : IScenarioSetUp, IScenarioTearDown
    {
        public bool ThrowOnSetUp { get; set; }
        public bool ThrowOnTearDown { get; set; }
        public bool SetUpCalled { get; private set; }
        public bool TearDownCalled { get; private set; }
        public bool PassCalled { get; private set; }

        public Task OnScenarioSetUp()
        {
            SetUpCalled = true;
            if (ThrowOnSetUp)
                throw new InvalidOperationException("OnSetUp");
            return Task.CompletedTask;
        }

        public Task OnScenarioTearDown()
        {
            TearDownCalled = true;
            if (ThrowOnTearDown)
                throw new InvalidOperationException("OnTearDown");
            return Task.CompletedTask;
        }

        public void Pass()
        {
            PassCalled = true;
        }

        public void Fail()
        {
            throw new IOException("IO");
        }
    }
}

