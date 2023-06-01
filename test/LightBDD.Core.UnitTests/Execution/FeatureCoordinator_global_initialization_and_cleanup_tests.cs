using System;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
[NonParallelizable]
public class FeatureCoordinator_global_initialization_and_cleanup_tests
{
    class TestableFeatureCoordinator : FrameworkFeatureCoordinator
    {
        public TestableFeatureCoordinator()
            : this(TestableIntegrationContextBuilder.Default()) { }

        public TestableFeatureCoordinator(TestableIntegrationContextBuilder builder)
            : base(builder.Build())
        {
        }

        public TestableFeatureCoordinator InstallSelf()
        {
            Install(this);
            return this;
        }
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

        public async Task CleanUpAsync()
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

        public async Task CleanUpAsync()
        {
            await Task.Yield();
            CleanUpSeq = _counter.Next();
        }
    }

    private TestableFeatureCoordinator SetupCoordinator(Action<LightBddConfiguration> cfg = null)
    {
        return new TestableFeatureCoordinator(TestableIntegrationContextBuilder.Default().WithConfiguration(cfg));
    }

    [Test]
    public void Runner_should_inject_dependencies_for_properties_with_FixtureDependency_attribute()
    {
        var counter = new Counter();
        var dep1 = new InitializableDependency(counter);
        var dep2 = new InitializableDependency2(counter);
        var setUpSeq = -1;
        var cleanUpSeq = -1;
        var cleanUpSeq2 = -1;
        var syncSetUpSeq = -1;
        var syncCleanUpSeq = -1;
        var syncCleanUpSeq2 = -1;

        async Task GlobalSetUp() => setUpSeq = counter.Next();
        async Task GlobalCleanUp() => cleanUpSeq = counter.Next();
        async Task GlobalCleanUp2() => cleanUpSeq2 = counter.Next();
        void SyncSetUp() => syncSetUpSeq = counter.Next();
        void SyncCleanUp() => syncCleanUpSeq = counter.Next();
        void SyncCleanUp2() => syncCleanUpSeq2 = counter.Next();

        void Configure(LightBddConfiguration cfg)
        {
            cfg.DependencyContainerConfiguration()
                .UseDefault(c =>
                {
                    c.RegisterInstance(counter);
                    c.RegisterInstance(dep1);
                    c.RegisterInstance(dep2);
                });

            cfg.ExecutionExtensionsConfiguration()
                .RegisterGlobalSetUp<InitializableDependency>()
                .RegisterGlobalSetUp<InitializableDependency2>()
                .RegisterGlobalSetUp("global1", GlobalSetUp, GlobalCleanUp)
                .RegisterGlobalCleanUp("global2", GlobalCleanUp2)
                .RegisterGlobalSetUp("sync1", SyncSetUp, SyncCleanUp)
                .RegisterGlobalCleanUp("sync2", SyncCleanUp2);
        }

        void AssertInitializablesAreConfigured()
        {
            Assert.That(dep1.InitializeSeq, Is.Not.EqualTo(-1));
            Assert.That(dep2.InitializeSeq, Is.Not.EqualTo(-1));
            Assert.That(setUpSeq, Is.Not.EqualTo(-1));
            Assert.That(syncSetUpSeq, Is.Not.EqualTo(-1));
        }

        using var coordinator = SetupCoordinator(Configure);
        coordinator.InstallSelf();

        var runner = coordinator.RunnerRepository.GetRunnerFor(GetType()).GetBddRunner(this);
        runner.Test().TestScenario(AssertInitializablesAreConfigured);
        Assert.That(dep1.InitializeSeq, Is.EqualTo(1));
        Assert.That(dep2.InitializeSeq, Is.EqualTo(2));
        Assert.That(setUpSeq, Is.EqualTo(3));
        Assert.That(syncSetUpSeq, Is.EqualTo(4));

        Assert.That(dep1.CleanUpSeq, Is.EqualTo(-1));
        Assert.That(dep2.CleanUpSeq, Is.EqualTo(-1));
        Assert.That(cleanUpSeq, Is.EqualTo(-1));
        Assert.That(cleanUpSeq2, Is.EqualTo(-1));
        Assert.That(syncCleanUpSeq, Is.EqualTo(-1));
        Assert.That(syncCleanUpSeq2, Is.EqualTo(-1));

        coordinator.Dispose();

        Assert.That(dep1.CleanUpSeq, Is.EqualTo(10));
        Assert.That(dep2.CleanUpSeq, Is.EqualTo(9));
        Assert.That(cleanUpSeq, Is.EqualTo(8));
        Assert.That(cleanUpSeq2, Is.EqualTo(7));
        Assert.That(syncCleanUpSeq, Is.EqualTo(6));
        Assert.That(syncCleanUpSeq2, Is.EqualTo(5));
    }
}