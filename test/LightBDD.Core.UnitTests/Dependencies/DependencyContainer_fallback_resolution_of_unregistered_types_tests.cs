using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.ScenarioHelpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Dependencies
{
    [TestFixture]
    public class DependencyContainer_fallback_resolution_of_unregistered_types_tests
    {
        [Test]
        public async Task Container_should_use_transient_fallback_resolution_behavior_for_unregistered_types()
        {
            await using var container = CreateContainer();
            Assert.AreNotEqual(container.Resolve<Disposable>(), container.Resolve<Disposable>());

            await using var inner = container.BeginScope();
            Assert.AreNotEqual(container.Resolve<Disposable>(), inner.Resolve<Disposable>());
        }

        [Test]
        public async Task Resolve_should_always_return_new_instance_and_dispose_it_upon_completion()
        {
            Disposable instance1;
            Disposable instance2;
            await using (var container = CreateContainer())
            {
                instance1 = container.Resolve<Disposable>();
                instance2 = container.Resolve<Disposable>();
                Assert.AreNotSame(instance1, instance2);
            }
            Assert.True(instance1.Disposed);
            Assert.True(instance2.Disposed);
        }

        [Test]
        public async Task It_should_throw_if_type_has_no_public_ctors()
        {
            await using var container = CreateContainer();
            var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<NoCtorType>());
            Assert.That(ex.Message, Is.EqualTo($"Unable to create transient instance of type '{typeof(NoCtorType)}':{Environment.NewLine}Type '{typeof(NoCtorType)}' has to have exactly one public constructor (number of public constructors: 0)."));
        }

        [Test]
        public async Task It_should_throw_if_type_is_interface_and_have_no_explicit_registrations()
        {
            await using var container = CreateContainer();
            var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<IDisposable>());
            Assert.That(ex.Message, Is.EqualTo($"Unable to create transient instance of type '{typeof(IDisposable)}':{Environment.NewLine}Type '{typeof(IDisposable)}' has to be non-abstract class or value type."));
        }

        [Test]
        public async Task It_should_throw_if_type_is_abstract_and_have_no_explicit_registrations()
        {
            await using var container = CreateContainer();
            var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<Abstract>());
            Assert.That(ex.Message, Is.EqualTo($"Unable to create transient instance of type '{typeof(Abstract)}':{Environment.NewLine}Type '{typeof(Abstract)}' has to be non-abstract class or value type."));
        }

        [Test]
        public async Task It_should_throw_if_type_has_multiple_public_ctors()
        {
            await using var container = CreateContainer();
            var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<MultiCtorType>());
            Assert.That(ex.Message, Is.EqualTo($"Unable to create transient instance of type '{typeof(MultiCtorType)}':{Environment.NewLine}Type '{typeof(MultiCtorType)}' has to have exactly one public constructor (number of public constructors: 2)."));
        }

        [Test]
        public async Task It_should_resolve_complex_types_with_parameterized_ctor()
        {
            Complex complex;
            await using (var container = CreateContainer())
            {
                complex = container.Resolve<Complex>();
                Assert.That(complex, Is.Not.Null);
                Assert.That(complex.Disposable, Is.Not.Null);
                Assert.That(complex.OtherComplex, Is.Not.Null);
                Assert.That(complex.OtherComplex.Disposable, Is.Not.Null);
                Assert.That(complex.AnotherComplex.Disposable, Is.Not.Null);

                Assert.That(complex.Disposable, Is.Not.SameAs(complex.OtherComplex.Disposable));
                Assert.That(complex.Disposable, Is.Not.SameAs(complex.AnotherComplex.Disposable));
            }
            Assert.That(complex.Disposable.Disposed, Is.True);
            Assert.That(complex.OtherComplex.Disposable.Disposed, Is.True);
        }

        [Test]
        public async Task It_should_resolve_complex_types_with_parameterized_ctors_honoring_registrations()
        {
            var disposable = new Disposable();
            var otherComplex = new OtherComplex(new Disposable());
            await using var container = CreateContainer(c =>
            {
                c.AddSingleton(disposable);
                c.AddSingleton(otherComplex);
            });
            await using var scope = container.BeginScope();

            var complex = scope.Resolve<Complex>();
            Assert.That(complex, Is.Not.Null);
            Assert.That(complex.Disposable, Is.SameAs(disposable));
            Assert.That(complex.OtherComplex, Is.SameAs(otherComplex));
        }

        [Test]
        public async Task Resolve_should_throw_meaningful_exception_if_type_cannot_be_resolved()
        {
            await using var container = CreateContainer();
            var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<Holder<ProblematicType>>());

            Assert.That(ex.Message.NormalizeNewLine(), Is.EqualTo($@"Unable to create transient instance of type '{typeof(Holder<ProblematicType>)}':
Unable to create transient instance of type '{typeof(ProblematicType)}':
Unable to create transient instance of type '{typeof(MultiCtorType)}':
Type '{typeof(MultiCtorType)}' has to have exactly one public constructor (number of public constructors: 2).".NormalizeNewLine()));
        }

        [Test]
        public void Dispose_should_throw_InvalidOperationException_on_disposal_failure()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await using var container = CreateContainer();
                container.Resolve<FaultyDisposable>();
            });

            Assert.That(ex.Message, Is.EqualTo($"Failed to dispose transient dependency '{typeof(FaultyDisposable).Name}': boom!"));
        }

        [Test]
        public void Dispose_should_throw_InvalidOperationException_on_async_disposal_failure()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await using var container = CreateContainer();
                container.Resolve<FaultyAsyncDisposable>();
            });

            Assert.That(ex.Message, Is.EqualTo($"Failed to dispose transient dependency '{typeof(FaultyAsyncDisposable).Name}': boom!"));
        }

        protected IDependencyContainer CreateContainer()
        {
            return CreateContainer(x => { });
        }

        private static IDependencyContainer CreateContainer(Action<IServiceCollection> configurator)
        {
            var cfg = new LightBddConfiguration();
            configurator?.Invoke(cfg.Services);
            return cfg.BuildContainer();
        }

        class FaultyDisposable : Disposable
        {
            public override void Dispose()
            {
                base.Dispose();
                throw new InvalidOperationException("boom!");
            }
        }

        class FaultyAsyncDisposable : AsyncDisposable
        {
            public override async ValueTask DisposeAsync()
            {
                await base.DisposeAsync();
                throw new InvalidOperationException("boom!");
            }
        }

        class Holder<T>
        {
            public T Value { get; }

            public Holder(T value)
            {
                Value = value;
            }
        }

        class ProblematicType
        {
            public ProblematicType(Disposable disposable, MultiCtorType param2)
            {
            }
        }

        class NoCtorType
        {
            protected NoCtorType() { }
        }
        class MultiCtorType
        {
            public MultiCtorType() { }
            public MultiCtorType(Disposable x) { }
        }
        abstract class Abstract
        {
        }

        class Complex
        {
            public Disposable Disposable { get; }
            public OtherComplex OtherComplex { get; }
            public AnotherComplex AnotherComplex { get; }

            public Complex(Disposable disposable, OtherComplex otherComplex, AnotherComplex anotherComplex)
            {
                Disposable = disposable;
                OtherComplex = otherComplex;
                AnotherComplex = anotherComplex;
            }
        }

        class OtherComplex
        {
            public Disposable Disposable { get; }
            public OtherComplex(Disposable disposable)
            {
                Disposable = disposable;
            }
        }

        class AnotherComplex
        {
            public Disposable Disposable { get; }

            public AnotherComplex(Disposable disposable)
            {
                Disposable = disposable;
            }
        }

        class Disposable : IDisposable
        {
            public virtual void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        class AsyncDisposable : IAsyncDisposable
        {
            public virtual ValueTask DisposeAsync()
            {
                Disposed = true;
                return default;
            }

            public bool Disposed { get; private set; }
        }
    }
}
