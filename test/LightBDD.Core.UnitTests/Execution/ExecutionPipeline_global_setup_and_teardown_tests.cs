using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
#pragma warning disable CS1998

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class ExecutionPipeline_global_setup_and_teardown_tests
{
    class MyFeature
    {
        [TestScenario]
        public Task MyScenario1() => TestScenarioBuilder.Current.TestScenario(
            TestStep.CreateNamed("step", () => ScenarioExecutionContext.CurrentScenario.DependencyResolver.Resolve<Counter>().Next()));
    }
    class Counter
    {
        private int _counter = 0;
        public int Next() => Interlocked.Increment(ref _counter);
    }

    class InitializableDependency : IGlobalResourceSetUp
    {
        private readonly Counter _counter;
        public int InitializeSeq { get; private set; } = -1;
        public int CleanUpSeq { get; private set; } = -1;

        public InitializableDependency(Counter counter)
        {
            _counter = counter;
        }

        public async Task SetUpAsync()
        {
            await Task.Yield();
            InitializeSeq = _counter.Next();
        }

        public async Task TearDownAsync()
        {
            await Task.Yield();
            CleanUpSeq = _counter.Next();
        }
    }

    class InitializableDependency2 : IGlobalResourceSetUp
    {
        private readonly Counter _counter;
        public int InitializeSeq { get; private set; } = -1;
        public int CleanUpSeq { get; private set; } = -1;

        public InitializableDependency2(Counter counter)
        {
            _counter = counter;
        }

        public async Task SetUpAsync()
        {
            await Task.Yield();
            InitializeSeq = _counter.Next();
        }

        public async Task TearDownAsync()
        {
            await Task.Yield();
            CleanUpSeq = _counter.Next();
        }
    }

    [Test]
    public async Task Coordinator_should_run_global_SetUps_on_installation_and_TearDowns_on_disposal()
    {
        var counter = new Counter();
        var dep1 = new InitializableDependency(counter);
        var dep2 = new InitializableDependency2(counter);
        var setUpSeq = -1;
        var cleanUpSeq = -1;
        var cleanUpSeq2 = -1;
        var syncSetUpSeq = -1;
        var syncSetUpSeq3 = -1;
        var syncCleanUpSeq = -1;
        var syncCleanUpSeq2 = -1;

        async Task GlobalSetUp() => setUpSeq = counter.Next();
        async Task GlobalCleanUp() => cleanUpSeq = counter.Next();
        async Task GlobalCleanUp2() => cleanUpSeq2 = counter.Next();
        void SyncSetUp() => syncSetUpSeq = counter.Next();
        void SyncCleanUp() => syncCleanUpSeq = counter.Next();
        void SyncCleanUp2() => syncCleanUpSeq2 = counter.Next();
        void SyncSetUp3() => syncSetUpSeq3 = counter.Next();

        void Configure(LightBddConfiguration cfg)
        {
            cfg.Services
                .AddSingleton(counter)
                .AddSingleton(dep1)
                .AddSingleton(dep2);

            cfg.Services
                .ConfigureGlobalSetUp()
                .RegisterGlobalSetUp<InitializableDependency>()
                .RegisterGlobalSetUp<InitializableDependency2>()
                .RegisterGlobalSetUp("global1", GlobalSetUp, GlobalCleanUp)
                .RegisterGlobalTearDown("global2", GlobalCleanUp2)
                .RegisterGlobalSetUp("sync1", SyncSetUp, SyncCleanUp)
                .RegisterGlobalTearDown("sync2", SyncCleanUp2)
                .RegisterGlobalSetUp("sync3", SyncSetUp3);
        }

        await TestableCoreExecutionPipeline.Create(Configure).ExecuteFeature(typeof(MyFeature));

        Assert.That(dep1.InitializeSeq, Is.EqualTo(1));
        Assert.That(dep2.InitializeSeq, Is.EqualTo(2));
        Assert.That(setUpSeq, Is.EqualTo(3));
        Assert.That(syncSetUpSeq, Is.EqualTo(4));
        Assert.That(syncSetUpSeq3, Is.EqualTo(5));

        //scenario counter

        Assert.That(syncCleanUpSeq2, Is.EqualTo(7));
        Assert.That(syncCleanUpSeq, Is.EqualTo(8));
        Assert.That(cleanUpSeq2, Is.EqualTo(9));
        Assert.That(cleanUpSeq, Is.EqualTo(10));
        Assert.That(dep2.CleanUpSeq, Is.EqualTo(11));
        Assert.That(dep1.CleanUpSeq, Is.EqualTo(12));
    }

    [Test]
    public async Task Only_TearDowns_for_successful_SetUps_should_run()
    {
        var counter = new Counter();
        var dep1 = new InitializableDependency(counter);
        var dep2 = new InitializableDependency2(counter);
        int setUpSeq = -1;
        int cleanUpSeq = -1;
        int setUpSeq2 = -1;
        int cleanUpSeq2 = -1;
        int cleanUpSeq3 = -1;
        int cleanUpSeq4 = -1;
        async Task GlobalSetUp() => setUpSeq = counter.Next();
        async Task GlobalCleanUp() => cleanUpSeq = counter.Next();
        async Task GlobalSetUp2() => setUpSeq2 = counter.Next();
        async Task GlobalCleanUp2() => cleanUpSeq2 = counter.Next();
        async Task GlobalCleanUp3() => cleanUpSeq3 = counter.Next();
        void GlobalCleanUp4() => cleanUpSeq4 = counter.Next();

        void Configure(LightBddConfiguration cfg)
        {
            cfg.Services
                .AddSingleton(counter)
                .AddSingleton(dep1)
                .AddSingleton(dep2);

            cfg.Services.ConfigureGlobalSetUp()
                .RegisterGlobalSetUp<InitializableDependency>()
                .RegisterGlobalSetUp("global1", GlobalSetUp, GlobalCleanUp)
                .RegisterGlobalSetUp("failing", () => throw new IOException("BOOM"))
                .RegisterGlobalSetUp<InitializableDependency2>()
                .RegisterGlobalSetUp("global2", GlobalSetUp2, GlobalCleanUp2)
                .RegisterGlobalTearDown("global3", GlobalCleanUp3)
                .RegisterGlobalTearDown("global4", GlobalCleanUp4);
        }

        var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(typeof(MyFeature));
        result.OverallStatus.ShouldBe(ExecutionStatus.Failed);

        Assert.That(dep1.InitializeSeq, Is.EqualTo(1));
        Assert.That(setUpSeq, Is.EqualTo(2));
        Assert.That(dep2.InitializeSeq, Is.EqualTo(-1));
        Assert.That(setUpSeq2, Is.EqualTo(-1));
        Assert.That(cleanUpSeq4, Is.EqualTo(3));
        Assert.That(cleanUpSeq3, Is.EqualTo(4));
        Assert.That(cleanUpSeq2, Is.EqualTo(-1));
        Assert.That(dep2.CleanUpSeq, Is.EqualTo(-1));
        Assert.That(cleanUpSeq, Is.EqualTo(5));
        Assert.That(dep1.CleanUpSeq, Is.EqualTo(6));
    }

    [Test]
    public async Task Failing_GlobalSetUp_should_fail_scenarios()
    {
        var counter = new Counter();
        void Configure(LightBddConfiguration cfg)
        {
            cfg.Services.AddSingleton(counter);

            cfg.Services.ConfigureGlobalSetUp()
                .RegisterGlobalSetUp("failing", () => throw new IOException("BOOM"));
        }

        var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(typeof(MyFeature));
        result.OverallStatus.ShouldBe(ExecutionStatus.Failed);
        result.Features.Single().GetScenarios().Single().StatusDetails.ShouldBe("Scenario: Global set up failed: Set up activity 'failing' failed: BOOM");
    }
}