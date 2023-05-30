using System;
using System.IO;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests;

[TestFixture]
public class CoreBddRunner_execution_extension_setup_and_teardown_tests
{
    private IFeatureRunner _feature;
    private Fixture _fixture;

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
                throw new InvalidOperationException("TearDown");
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
        _feature = TestableFeatureRunnerRepository.GetRunner(typeof(Fixture));
        _fixture = new Fixture();
    }

    [Test]
    public void Runner_should_call_OnScenarioSetUp_and_OnScenarioTearDown_when_provided()
    {
        _feature.GetBddRunner(_fixture).Test().TestScenario(_fixture.Pass);
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
    }

    [Test]
    public void Runner_should_call_OnScenarioTearDown_on_step_failure()
    {
        Assert.Throws<IOException>(() => _feature.GetBddRunner(_fixture).Test().TestScenario(_fixture.Fail));
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
    }

    [Test]
    public void Runner_should_call_OnScenarioTearDown_on_SetUp_failure_and_propagate_SetUp_exception_without_calling_steps()
    {
        _fixture.ThrowOnSetUp = true;
        var ex = Assert.Throws<InvalidOperationException>(() => _feature.GetBddRunner(_fixture).Test().TestScenario(_fixture.Pass));
        Assert.That(ex.Message, Is.EqualTo("OnSetUp"));
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
        Assert.That(_fixture.PassCalled, Is.False);
    }

    [Test]
    public void Runner_should_propagate_steps_and_TearDown_exceptions()
    {
        _fixture.ThrowOnTearDown = true;
        var ex = Assert.Throws<AggregateException>(() => _feature.GetBddRunner(_fixture).Test().TestScenario(_fixture.Fail));
        Assert.That(ex.Message, Is.EqualTo("One or more errors occurred. (TearDown) (IO)"));
        Assert.That(_fixture.SetUpCalled, Is.True);
        Assert.That(_fixture.TearDownCalled, Is.True);
    }
}