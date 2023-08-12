using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Dependencies;

public class DependencyContainer_tests
{
    [Test]
    public async Task Disposal_of_container_should_dispose_instances()
    {
        DisposableSingleton singleton;
        DisposableScoped scoped;
        DisposableTransient transient;
        await using (var container = CreateContainer())
        {
            await using var scope = container.BeginScope();
            singleton = scope.Resolve<DisposableSingleton>();
            scoped = scope.Resolve<DisposableScoped>();
            transient = scope.Resolve<DisposableTransient>();
            Assert.False(singleton.Disposed);
            Assert.False(scoped.Disposed);
            Assert.False(transient.Disposed);
        }
        Assert.True(singleton.Disposed);
        Assert.True(scoped.Disposed);
        Assert.True(transient.Disposed);
    }

    [Test]
    public async Task BeginScope_should_make_separate_scope()
    {
        DisposableTransient outer;
        DisposableTransient inner;
        DisposableTransient deepestInner;
        await using (var container = CreateContainer())
        {
            outer = container.Resolve<DisposableTransient>();
            await using (var scope = container.BeginScope())
            {
                inner = scope.Resolve<DisposableTransient>();
                await using (var deepestScope = scope.BeginScope())
                    deepestInner = deepestScope.Resolve<DisposableTransient>();
                Assert.True(deepestInner.Disposed);
                Assert.False(inner.Disposed);
                Assert.False(outer.Disposed);
            }

            Assert.True(inner.Disposed);
            Assert.False(outer.Disposed);
        }
        Assert.True(outer.Disposed);
    }

    [Test]
    public async Task It_should_resolve_current_container()
    {
        await using var container = CreateContainer();
        await using var scope = container.BeginScope();
        Assert.That(scope.Resolve<IDependencyContainer>(), Is.SameAs(scope));
        Assert.That(scope.Resolve<IDependencyResolver>(), Is.SameAs(scope));
        Assert.That(container.Resolve<IDependencyContainer>(), Is.SameAs(container));
        Assert.That(container.Resolve<IDependencyResolver>(), Is.SameAs(container));
    }

    [Test]
    public async Task Resolve_should_honor_scopes()
    {
        await using var container = CreateContainer();
        await using var scope1 = container.BeginScope();
        await using var scope2 = container.BeginScope();

        Assert.AreSame(container.Resolve<DisposableSingleton>(), scope1.Resolve<DisposableSingleton>());
        Assert.AreSame(scope1.Resolve<DisposableScoped>(), scope1.Resolve<DisposableScoped>());
        Assert.AreNotSame(scope1.Resolve<DisposableScoped>(), scope2.Resolve<DisposableScoped>());
        Assert.AreNotSame(scope1.Resolve<DisposableTransient>(), scope1.Resolve<DisposableTransient>());
        Assert.AreNotSame(scope2.Resolve<DisposableTransient>(), scope2.Resolve<DisposableTransient>());
        Assert.AreNotSame(container.Resolve<DisposableTransient>(), container.Resolve<DisposableTransient>());
    }

    [Test]
    public async Task Resolve_should_be_thread_safe()
    {
        await using var container = new LightBddConfiguration()
            .ConfigureDependencies(x =>
            {
                x.AddSingleton<SlowDependency>();
                x.AddSingleton<object>(x => x.GetRequiredService<SlowDependency>());
            })
            .BuildContainer();

        await using var scenario = container.BeginScope();

        var all = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Task.Run(() => (object)scenario.Resolve<SlowDependency>()))
            .Concat(Enumerable.Range(0, 10).Select(_ => Task.Run(() => container.Resolve<object>()))));
        Assert.AreEqual(20, all.Length);
        Assert.AreEqual(1, all.Distinct().Count());
        Assert.AreEqual(1, SlowDependency.Instances);
    }

    private IDependencyContainer CreateContainer() => new LightBddConfiguration()
        .ConfigureDependencies(c => c
            .AddSingleton<DisposableSingleton>()
            .AddScoped<DisposableScoped>()
            .AddTransient<DisposableTransient>())
        .BuildContainer();

    class DisposableSingleton : IDisposable
    {
        public virtual void Dispose()
        {
            Disposed = true;
        }

        public bool Disposed { get; private set; }
    }

    class DisposableScoped : IDisposable
    {
        public virtual void Dispose()
        {
            Disposed = true;
        }

        public bool Disposed { get; private set; }
    }

    class DisposableTransient : IDisposable
    {
        public virtual void Dispose()
        {
            Disposed = true;
        }

        public bool Disposed { get; private set; }
    }

    class SlowDependency
    {
        public static int Instances = 0;
        public SlowDependency()
        {
            Thread.Sleep(1000);
            Interlocked.Increment(ref Instances);
        }
    }
}