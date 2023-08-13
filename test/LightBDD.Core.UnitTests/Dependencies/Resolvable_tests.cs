#nullable enable
using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Dependencies
{
    [TestFixture]
    public class Resolvable_tests
    {

        [Test]
        public void It_should_resolve_configured_instance()
        {
            var disposable = Mock.Of<IDisposable>();
            Resolvable.Use(disposable).Resolve(Mock.Of<IDependencyResolver>())
                .ShouldBeSameAs(disposable);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task It_should_resolve_dependency_with_factory_method_and_honor_takeOwnership_flag(bool shouldTakeOwnership)
        {
            Disposable? disposable;
            await using var container = CreateContainer();
            await using (var scope = container.BeginScope())
            {
                disposable = Resolvable.Use(() => new Disposable(), shouldTakeOwnership).Resolve(scope);
                disposable.IsDisposed.ShouldBeFalse();
            }
            disposable.IsDisposed.ShouldBe(shouldTakeOwnership);
        }

        [Test]
        public async Task It_should_resolve_dependency_from_container()
        {
            var disposable = new Disposable();
            await using var container = CreateContainer(x => x.AddSingleton(disposable));
            var dependency = Resolvable.Default<Dependency>().Resolve(container);
            dependency.Disposable.ShouldBeSameAs(disposable);
        }

        [Test]
        public async Task It_should_resolve_dependency_from_container_with_explicit_method()
        {
            var disposable = new Disposable();
            await using var container = CreateContainer(x => x.AddSingleton(disposable));
            var dependency = Resolvable.Use(r => r.Resolve<Dependency>()).Resolve(container);
            dependency.Disposable.ShouldBeSameAs(disposable);
        }

        [Test]
        public void It_should_resolve_null()
        {
            Resolvable.Null<Dependency>().Resolve(Mock.Of<IDependencyResolver>()).ShouldBeNull();
        }

        private IDependencyContainer CreateContainer(Action<IServiceCollection> onCreate = null) => new LightBddConfiguration()
            .ConfigureDependencies(onCreate)
            .BuildContainer();

        class Dependency
        {
            public Disposable Disposable { get; }

            public Dependency(Disposable disposable)
            {
                Disposable = disposable;
            }
        }

        class Disposable : IDisposable
        {
            public bool IsDisposed { get; private set; }
            public void Dispose() => IsDisposed = true;
        }
    }
}
