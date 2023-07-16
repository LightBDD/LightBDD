using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests;

[TestFixture]
public class CoreBddRunner_execution_extension_setup_and_teardown_tests
{
    private Fixture _fixture;
    private ICoreScenarioBuilder CreateScenarioBuilder() => TestableExecutionPipeline.Default.CreateScenarioBuilder(_fixture);

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

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
    }

    [Test]
    public void Runner_should_call_OnScenarioSetUp_and_OnScenarioTearDown_when_provided()
    {
        CreateScenarioBuilder().Test().TestScenario(_fixture.Pass);
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
    }

    [Test]
    public void Runner_should_call_OnScenarioTearDown_on_step_failure()
    {
        Assert.Throws<IOException>(() => CreateScenarioBuilder().Test().TestScenario(_fixture.Fail));
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
    }

    [Test]
    public void Runner_should_call_OnScenarioTearDown_on_SetUp_failure_and_propagate_SetUp_exception_without_calling_steps()
    {
        _fixture.ThrowOnSetUp = true;
        var ex = Assert.Throws<InvalidOperationException>(() => CreateScenarioBuilder().Test().TestScenario(_fixture.Pass));
        Assert.That(ex.Message, Is.EqualTo("OnSetUp"));
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
        Assert.That(_fixture.PassCalled, Is.False);
    }

    [Test]
    public void Runner_should_propagate_steps_and_TearDown_exceptions()
    {
        _fixture.ThrowOnTearDown = true;
        var ex = Assert.Throws<AggregateException>(() => CreateScenarioBuilder().Test().TestScenario(_fixture.Fail));
        Assert.That(ex.Message, Does.Contain("One or more errors occurred."));
        Assert.That(ex.InnerExceptions.Select(e => e.Message).ToArray(), Is.EquivalentTo(new[] { "IO", "OnTearDown" }));
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
    }
}