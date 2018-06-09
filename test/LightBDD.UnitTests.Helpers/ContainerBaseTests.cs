using System;
using LightBDD.Core.Dependencies;
using Moq;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    public abstract class ContainerBaseTests
    {
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
                    options.ExtenrallyOwned();

                using (var scope = container.BeginScope(cfg => cfg.RegisterInstance(instance.Object, options)))
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
        public void BeginScope_should_make_separate_scope()
        {
            Disposable outer;
            Disposable inner;
            using (var container = CreateContainer())
            {
                outer = container.Resolve<Disposable>();
                using (var scope = container.BeginScope())
                    inner = scope.Resolve<Disposable>();

                Assert.True(inner.Disposed);
                Assert.False(outer.Disposed);
            }
            Assert.True(outer.Disposed);
            Assert.True(inner.Disposed);
        }

        [Test]
        public void BeginScope_should_allow_adding_instances()
        {
            var instance = new Disposable();
            using (var container = CreateContainer())
            {
                using (var scope = container.BeginScope(cfg => cfg.RegisterInstance(instance, new RegistrationOptions().As<Disposable>())))
                    Assert.That(scope.Resolve<Disposable>(), Is.SameAs(instance));

                Assert.True(instance.Disposed);
            }
        }

        [Test]
        public void It_should_resolve_current_container()
        {
            using (var container = CreateContainer())
            {
                using (var scope = container.BeginScope())
                {
                    Assert.That(scope.Resolve<IDependencyContainer>(), Is.SameAs(scope));
                    Assert.That(container.Resolve<IDependencyContainer>(), Is.SameAs(container));
                }
            }
        }

        [Test]
        public void RegisterInstance_should_honor_types()
        {
            var disposable = new Disposable();
            using (var container = CreateContainer())
            {
                using (var scope = container.BeginScope(c => c.RegisterInstance(disposable,
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
            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }
    }
}