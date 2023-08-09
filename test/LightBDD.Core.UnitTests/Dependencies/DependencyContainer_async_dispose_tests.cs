using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Dependencies;

public class DependencyContainer_async_dispose_tests
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

    private IDependencyContainer CreateContainer() => new DependencyContainerConfiguration()
        .ConfigureServices(c => c
            .AddSingleton<DisposableSingleton>()
            .AddScoped<DisposableScoped>()
            .AddTransient<DisposableTransient>())
        .Build();

    class DisposableSingleton : IAsyncDisposable
    {
        public virtual ValueTask DisposeAsync()
        {
            Disposed = true;
            return default;
        }

        public bool Disposed { get; private set; }
    }

    class DisposableScoped : IAsyncDisposable
    {
        public virtual ValueTask DisposeAsync()
        {
            Disposed = true;
            return default;
        }

        public bool Disposed { get; private set; }
    }

    class DisposableTransient : IAsyncDisposable
    {
        public virtual ValueTask DisposeAsync()
        {
            Disposed = true;
            return default;
        }

        public bool Disposed { get; private set; }
    }
}