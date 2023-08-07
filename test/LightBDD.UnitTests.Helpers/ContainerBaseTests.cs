using System;
using LightBDD.Core.Dependencies;
using Moq;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    public abstract class ContainerBaseTests
    {
        protected readonly bool ShouldTakeOwnership;

        protected ContainerBaseTests(bool shouldTakeOwnership = true)
        {
            ShouldTakeOwnership = shouldTakeOwnership;
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void RegisterInstance_should_handle_disposal_accordingly(bool expectDispose)
        {
            var instance = new Mock<IDisposable>();
            using (var container = CreateContainer())
            {
                var options = new RegistrationOptions().As(instance.Object.GetType());
                if (!expectDispose)
                    options.ExternallyOwned();

                using (var scope = container.BeginScope(LifetimeScope.Local, cfg => cfg.RegisterInstance(instance.Object, options)))
                    Assert.AreSame(instance.Object, scope.Resolve(instance.Object.GetType()));
            }

            instance.Verify(x => x.Dispose(), Times.Exactly(expectDispose ? 1 : 0));
        }

        [Test]
        public void Disposal_of_container_should_dispose_instances()
        {
            Disposable instance;
            using (var container = CreateContainer())
            {
                instance = container.Resolve<Disposable>();
                Assert.False(instance.Disposed);
            }
            Assert.True(instance.Disposed);
        }

        [Test]
        public void Container_ownership_should_control_disposal_of_root_level_singletons()
        {
            DisposableSingleton instance;
            using (var container = CreateContainer())
            {
                instance = container.Resolve<DisposableSingleton>();
                Assert.False(instance.Disposed);
            }
            Assert.That(instance.Disposed, Is.EqualTo(ShouldTakeOwnership));
        }

        [Test]
        public void Container_should_resolve_singleton_instances()
        {
            using (var container = CreateContainer())
            {
                Assert.That(
                    container.Resolve<DisposableSingleton>(),
                    Is.SameAs(container.Resolve<DisposableSingleton>()));
            }
        }

        [Test]
        public void BeginScope_should_make_separate_scope()
        {
            Disposable outer;
            Disposable inner;
            Disposable deepestInner;
            using (var container = CreateContainer())
            {
                outer = container.Resolve<Disposable>();
                using (var scope = container.BeginScope(LifetimeScope.Local))
                {
                    inner = scope.Resolve<Disposable>();
                    using (var deepestScope = scope.BeginScope(LifetimeScope.Local))
                        deepestInner = deepestScope.Resolve<Disposable>();
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
        public void BeginScope_should_allow_adding_instances()
        {
            var instance = new Disposable();
            using (var container = CreateContainer())
            {
                using (var scope = container.BeginScope(LifetimeScope.Local, cfg => cfg.RegisterInstance(instance, new RegistrationOptions().As<Disposable>())))
                    Assert.That(scope.Resolve<Disposable>(), Is.SameAs(instance));

                Assert.True(instance.Disposed);
            }
        }

        [Test]
        public void It_should_resolve_current_container()
        {
            using (var container = CreateContainer())
            {
                using (var scope = container.BeginScope(LifetimeScope.Local))
                {
                    Assert.That(scope.Resolve<IDependencyContainer>(), Is.SameAs(scope));
                    Assert.That(scope.Resolve<IDependencyResolver>(), Is.SameAs(scope));
                    Assert.That(container.Resolve<IDependencyContainer>(), Is.SameAs(container));
                    Assert.That(container.Resolve<IDependencyResolver>(), Is.SameAs(container));
                }
            }
        }

        [Test]
        public void RegisterInstance_should_honor_types()
        {
            var disposable = new Disposable();
            using (var container = CreateContainer())
            {
                using (var scope = container.BeginScope(LifetimeScope.Local, c => c.RegisterInstance(disposable,
                    new RegistrationOptions().As<Disposable>().As<IDisposable>().As<object>())))
                {
                    Assert.That(scope.Resolve<object>(), Is.SameAs(disposable));
                    Assert.That(scope.Resolve<IDisposable>(), Is.SameAs(disposable));
                    Assert.That(scope.Resolve<Disposable>(), Is.SameAs(disposable));
                }
            }
        }

        protected abstract IDependencyContainer CreateContainer();

        protected class Disposable : IDisposable
        {
            public virtual void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        protected class Disposable1 : IDisposable
        {
            public virtual void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        protected class Disposable2 : IDisposable
        {
            public virtual void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        protected class Disposable3 : IDisposable
        {
            public virtual void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        protected class Disposable4 : IDisposable
        {
            public virtual void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        protected class DisposableSingleton : IDisposable
        {
            public virtual void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }
    }
}