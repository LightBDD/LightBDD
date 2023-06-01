using System;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
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

    class InitializableDependency
    {
        private readonly Counter _counter;
        public int InitializeSeq { get; private set; } = -1;
        public int CleanUpSeq { get; private set; } = -1;

        public InitializableDependency(Counter counter)
        {
            _counter = counter;
        }

        public async Task InitializeAsync()
        {
            await Task.Yield();
            InitializeSeq = _counter.Next();
        }

        public async Task CleanupAsync()
        {
            await Task.Yield();
            CleanUpSeq = _counter.Next();
        }
    }

    class InitializableDependency2
    {
        private readonly Counter _counter;
        public int InitializeSeq { get; private set; } = -1;
        public int CleanUpSeq { get; private set; } = -1;

        public InitializableDependency2(Counter counter)
        {
            _counter = counter;
        }

        public async Task InitializeAsync()
        {
            await Task.Yield();
            InitializeSeq = _counter.Next();
        }

        public async Task CleanupAsync()
        {
            await Task.Yield();
            CleanUpSeq = _counter.Next();
        }
    }

    class CleanableDependency
    {
        private readonly Counter _counter;
        public int CleanUpSeq { get; private set; } = -1;

        public CleanableDependency(Counter counter)
        {
            _counter = counter;
        }

        public async Task CleanupAsync()
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
        var cleanable = new CleanableDependency(counter);
        var setUpFnSeq = -1;
        var cleanUpFnSeq = -1;

        Task GlobalSetUp()
        {
            setUpFnSeq = counter.Next();
            return Task.CompletedTask;
        }

        Task GlobalCleanUp()
        {
            cleanUpFnSeq = counter.Next();
            return Task.CompletedTask;
        }

        void Configure(LightBddConfiguration cfg)
        {
            cfg.DependencyContainerConfiguration()
                .UseDefault(c =>
                {
                    c.RegisterInstance(counter);
                    c.RegisterInstance(dep1);
                    c.RegisterInstance(dep2);
                    c.RegisterInstance(cleanable);
                });

            cfg.ExecutionExtensionsConfiguration()
                .RegisterGlobalSetUp<InitializableDependency>(x => x.InitializeAsync(), x => x.CleanupAsync())
                .RegisterGlobalSetUp<InitializableDependency2>(x => x.InitializeAsync(), x => x.CleanupAsync())
                .RegisterGlobalCleanUp<CleanableDependency>(x => x.CleanupAsync())
                .RegisterGlobalSetUp(GlobalSetUp)
                .RegisterGlobalCleanUp(GlobalCleanUp);
        }

        void AssertInitializablesAreConfigured()
        {
            Assert.That(dep1.InitializeSeq, Is.Not.EqualTo(-1));
            Assert.That(dep2.InitializeSeq, Is.Not.EqualTo(-1));
            Assert.That(setUpFnSeq, Is.Not.EqualTo(-1));
        }

        using var coordinator = SetupCoordinator(Configure);
        coordinator.InstallSelf();

        var runner = coordinator.RunnerRepository.GetRunnerFor(GetType()).GetBddRunner(this);
        runner.Test().TestScenario(AssertInitializablesAreConfigured);
        Assert.That(dep1.InitializeSeq, Is.EqualTo(1));
        Assert.That(dep2.InitializeSeq, Is.EqualTo(2));
        Assert.That(setUpFnSeq, Is.EqualTo(3));

        Assert.That(dep1.CleanUpSeq, Is.EqualTo(-1));
        Assert.That(dep2.CleanUpSeq, Is.EqualTo(-1));
        Assert.That(cleanUpFnSeq, Is.EqualTo(-1));

        coordinator.Dispose();

        Assert.That(cleanUpFnSeq, Is.EqualTo(4));
        Assert.That(cleanable.CleanUpSeq, Is.EqualTo(5));
        Assert.That(dep2.CleanUpSeq, Is.EqualTo(6));
        Assert.That(dep1.CleanUpSeq, Is.EqualTo(7));
    }
}