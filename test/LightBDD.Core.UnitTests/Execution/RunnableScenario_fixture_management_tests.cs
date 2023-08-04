using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class RunnableScenario_fixture_management_tests
{
    [Test]
    public async Task It_should_create_and_dispose_feature_fixture_instance()
    {
        TestableFixture captured = null;
        var feature = new TestResults.TestFeatureInfo { FeatureType = typeof(TestableFixture) };

        var result = await TestableScenarioFactory.Default.CreateBuilder(feature)
            .WithScenarioEntryMethod((fixture, _) =>
            {
                captured = (TestableFixture)fixture;
                captured.ShouldNotBeNull();
                captured.Disposed.ShouldBeFalse();
                return Task.CompletedTask;
            })
            .Build().RunAsync();

        result.Status.ShouldBe(ExecutionStatus.Passed);
        captured.ShouldNotBeNull();
        captured.Disposed.ShouldBeTrue();
    }

    [Test]
    public async Task It_should_create_and_dispose_feature_fixture_instance_upon_failure()
    {
        TestableFixture captured = null;
        var feature = new TestResults.TestFeatureInfo { FeatureType = typeof(TestableFixture) };

        var result = await TestableScenarioFactory.Default.CreateBuilder(feature)
            .WithScenarioEntryMethod((fixture, _) =>
            {
                captured = (TestableFixture)fixture;
                throw new Exception("failure");
            })
            .Build().RunAsync();

        result.Status.ShouldBe(ExecutionStatus.Failed);
        captured.ShouldNotBeNull();
        captured.Disposed.ShouldBeTrue();
    }

    [Test]
    public async Task It_should_capture_fixture_creation_failure()
    {
        var feature = new TestResults.TestFeatureInfo { FeatureType = typeof(BrokenFixture) };
        var result = await TestableScenarioFactory.Default.CreateBuilder(feature).Build().RunAsync();
        result.Status.ShouldBe(ExecutionStatus.Failed);
        result.ExecutionException.ShouldBeOfType<InvalidOperationException>().Message.ShouldBe("Initialization of BrokenFixture fixture failed: Something went wrong");
        result.StatusDetails.ShouldBe("Scenario Failed: System.InvalidOperationException: Initialization of BrokenFixture fixture failed: Something went wrong");
    }

    [Test]
    public async Task It_should_capture_OnScenarioSetUp_failure_and_dispose_fixture()
    {
        FixtureWithBrokenScenarioSetUp.Capture.Value = new Holder<bool>();

        var feature = new TestResults.TestFeatureInfo { FeatureType = typeof(FixtureWithBrokenScenarioSetUp) };
        var result = await TestableScenarioFactory.Default.CreateBuilder(feature).Build().RunAsync();
        result.Status.ShouldBe(ExecutionStatus.Failed);
        result.ExecutionException.ShouldBeOfType<InvalidOperationException>().Message.ShouldBe("OnScenarioSetUp() failed: boom");
        result.StatusDetails.ShouldBe("Scenario Failed: System.InvalidOperationException: OnScenarioSetUp() failed: boom");

        FixtureWithBrokenScenarioSetUp.Capture.Value.Value.ShouldBe(true);
    }

    [Test]
    public async Task It_should_capture_all_fixture_cleanup_failures()
    {
        var feature = new TestResults.TestFeatureInfo { FeatureType = typeof(FixtureWithBrokenCleanup) };
        var result = await TestableScenarioFactory.Default.CreateBuilder(feature).Build().RunAsync();
        result.Status.ShouldBe(ExecutionStatus.Failed);
        result.ExecutionException.ShouldBeOfType<AggregateException>()
            .InnerExceptions.Select(e => e.Message).ShouldBe(new[]
            {
                "Fixture OnScenarioTearDown() failed: boom",
                "Fixture DisposeAsync() failed: boom!",
                "Fixture Dispose() failed: boom!!"
            });
        result.StatusDetails.ShouldStartWith(
            "Scenario Failed: System.InvalidOperationException: Fixture OnScenarioTearDown() failed: boom" + Environment.NewLine
            + "\tSystem.InvalidOperationException: Fixture DisposeAsync() failed: boom!" + Environment.NewLine
            + "\tSystem.InvalidOperationException: Fixture Dispose() failed: boom!!");
    }

    class Holder<T>
    {
        public T Value;
    }

    class FixtureWithBrokenScenarioSetUp : IScenarioSetUp, IDisposable
    {
        public static readonly AsyncLocal<Holder<bool>> Capture = new();
        public Task OnScenarioSetUp() => throw new InvalidOperationException("boom");

        public void Dispose()
        {
            Capture.Value!.Value = true;
        }
    }

    class BrokenFixture
    {
        public BrokenFixture() => throw new Exception("Something went wrong");
    }

    class TestableFixture : IDisposable
    {
        public void Dispose()
        {
            Disposed = true;
        }

        public bool Disposed { get; private set; }
    }

    class FixtureWithBrokenCleanup : IScenarioTearDown, IAsyncDisposable, IDisposable
    {
        public Task OnScenarioTearDown()
        {
            throw new Exception("boom");
        }

        public ValueTask DisposeAsync()
        {
            throw new Exception("boom!");
        }

        public void Dispose()
        {
            throw new Exception("boom!!");
        }
    }
}
