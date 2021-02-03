using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Dependencies
{
    [TestFixture]
    public class BasicDependencyContainer_tests : ContainerBaseTests
    {
        [Test]
        public void Resolve_should_always_return_new_instance_and_dispose_upon_completion()
        {
            Disposable instance1;
            Disposable instance2;
            using (var container = CreateContainer())
            {
                instance1 = container.Resolve<Disposable>();
                instance2 = container.Resolve<Disposable>();
                Assert.AreNotSame(instance1, instance2);
            }
            Assert.True(instance1.Disposed);
            Assert.True(instance2.Disposed);
        }

        [Test]
        public void RegisterInstance_should_not_allow_to_register_as_invalid_type()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.BeginScope(c => c.RegisterInstance(new Disposable(), new RegistrationOptions().As<string>())));
                Assert.That(ex.Message, Is.EqualTo($"Type {typeof(Disposable)} is not assignable to {typeof(string)}"));
            }
        }

        [Test]
        public void It_should_throw_if_type_has_no_public_ctors()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<NoCtorType>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(NoCtorType)}:{Environment.NewLine}Type '{typeof(NoCtorType)}' has to have have exactly one public constructor (number of public constructors: 0)."));
            }
        }

        [Test]
        public void It_should_throw_if_type_is_interface_and_have_no_explicit_registrations()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<IDisposable>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(IDisposable)}:{Environment.NewLine}Type '{typeof(IDisposable)}' has to be non-abstract class or value type."));
            }
        }

        [Test]
        public void It_should_throw_if_type_is_abstract_and_have_no_explicit_registrations()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<Abstract>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(Abstract)}:{Environment.NewLine}Type '{typeof(Abstract)}' has to be non-abstract class or value type."));
            }
        }

        [Test]
        public void It_should_throw_if_type_has_multiple_public_ctors()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<MultiCtorType>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(MultiCtorType)}:{Environment.NewLine}Type '{typeof(MultiCtorType)}' has to have have exactly one public constructor (number of public constructors: 2)."));
            }
        }

        [Test]
        public void It_should_resolve_complex_types_with_parameterized_ctor()
        {
            Complex complex;
            using (var container = CreateContainer())
            {
                complex = container.Resolve<Complex>();
                Assert.That(complex, Is.Not.Null);
                Assert.That(complex.Disposable, Is.Not.Null);
                Assert.That(complex.OtherComplex, Is.Not.Null);
                Assert.That(complex.OtherComplex.Disposable, Is.Not.Null);
                Assert.That(complex.Value.Disposable, Is.Not.Null);

                Assert.That(complex.Disposable, Is.Not.SameAs(complex.OtherComplex.Disposable));
                Assert.That(complex.Disposable, Is.Not.SameAs(complex.Value.Disposable));
            }
            Assert.That(complex.Disposable.Disposed, Is.True);
            Assert.That(complex.OtherComplex.Disposable.Disposed, Is.True);
        }

        [Test]
        public void It_should_resolve_complex_types_with_parameterized_ctors_honoring_singletons_registered_across_scopes()
        {
            var outerDisposable = new Disposable();
            var innerDisposable = new Disposable();
            var otherComplex = new OtherComplex(new Disposable());
            using (var container = CreateContainer())
            {
                using (var outerScope = container.BeginScope(c =>
                     {
                         c.RegisterInstance(outerDisposable, new RegistrationOptions());
                         c.RegisterInstance(otherComplex, new RegistrationOptions());
                     }))
                {
                    using (var innerScope = outerScope.BeginScope(c => c.RegisterInstance(innerDisposable, new RegistrationOptions())))
                    {
                        var complex = innerScope.Resolve<Complex>();
                        Assert.That(complex, Is.Not.Null);
                        Assert.That(complex.Disposable, Is.SameAs(innerDisposable));
                        Assert.That(complex.OtherComplex, Is.SameAs(otherComplex));
                    }
                }
            }
        }

        [Test]
        public void Dispose_should_dispose_all_dependencies_and_throw_AggregateException_on_failure()
        {
            var items = new List<Disposable>();
            var ex = Assert.Throws<AggregateException>(() =>
            {
                using (var container = CreateContainer())
                {
                    items.Add(container.Resolve<Disposable>());
                    items.Add(container.Resolve<FaultyDisposable>());
                    items.Add(container.Resolve<FaultyDisposable>());
                    items.Add(container.Resolve<Disposable>());
                }
            });

            Assert.That(items.Count(x => x.Disposed), Is.EqualTo(4));
            Assert.That(ex.Message, Does.StartWith("Failed to dispose dependencies"));
            Assert.That(ex.InnerExceptions.Select(x => x.Message).ToArray(), Is.EqualTo(new[] { $"Failed to dispose dependency '{typeof(FaultyDisposable).Name}': boom!", $"Failed to dispose dependency '{typeof(FaultyDisposable).Name}': boom!" }));
        }

        [Test]
        public void Dispose_should_InvalidOperationException_on_disposal_failure()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                using (var container = CreateContainer())
                    container.Resolve<FaultyDisposable>();
            });

            Assert.That(ex.Message, Is.EqualTo($"Failed to dispose dependency '{typeof(FaultyDisposable).Name}': boom!"));
        }

        [Test]
        public void Resolve_should_throw_meaningful_exception_if_type_cannot_be_resolved()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<Holder<ProblematicType>>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(Holder<ProblematicType>)}:{Environment.NewLine}Unable to resolve type {typeof(ProblematicType)}:{Environment.NewLine}Unable to resolve type {typeof(MultiCtorType)}:{Environment.NewLine}Type '{typeof(MultiCtorType)}' has to have have exactly one public constructor (number of public constructors: 2)."));
            }
        }

        [Test]
        public void Resolve_should_honor_scopes()
        {
            using (var container = CreateContainer(x =>
            {
                x.RegisterSingleton<Disposable>();
                x.RegisterScenarioScoped<Disposable1>();
                x.RegisterLocallyScoped<Disposable2>();
                x.RegisterTransient<Disposable3>();
            }))
            {
                Assert.AreSame(container.Resolve<Disposable2>(), container.Resolve<Disposable2>());
                using (var scenario = container.BeginScope(LifetimeScope.Scenario))
                {
                    Assert.AreSame(container.Resolve<Disposable>(), scenario.Resolve<Disposable>());
                    Assert.AreSame(scenario.Resolve<Disposable1>(), scenario.Resolve<Disposable1>());
                    Assert.AreSame(scenario.Resolve<Disposable2>(), scenario.Resolve<Disposable2>());
                    Assert.AreNotSame(container.Resolve<Disposable2>(), scenario.Resolve<Disposable2>());
                    Assert.AreNotSame(scenario.Resolve<Disposable3>(), scenario.Resolve<Disposable3>());

                    using (var stepA = scenario.BeginScope(LifetimeScope.Local))
                    {
                        Assert.AreSame(container.Resolve<Disposable>(), stepA.Resolve<Disposable>());
                        Assert.AreSame(scenario.Resolve<Disposable1>(), stepA.Resolve<Disposable1>());
                        Assert.AreNotSame(scenario.Resolve<Disposable2>(), stepA.Resolve<Disposable2>());
                        Assert.AreSame(stepA.Resolve<Disposable2>(), stepA.Resolve<Disposable2>());
                        Assert.AreNotSame(stepA.Resolve<Disposable3>(), stepA.Resolve<Disposable3>());

                        using (var stepB = stepA.BeginScope(LifetimeScope.Local))
                        {
                            Assert.AreSame(container.Resolve<Disposable>(), stepB.Resolve<Disposable>());
                            Assert.AreSame(scenario.Resolve<Disposable1>(), stepB.Resolve<Disposable1>());
                            Assert.AreNotSame(stepA.Resolve<Disposable2>(), stepB.Resolve<Disposable2>());
                            Assert.AreSame(stepB.Resolve<Disposable2>(), stepB.Resolve<Disposable2>());
                            Assert.AreNotSame(stepB.Resolve<Disposable3>(), stepB.Resolve<Disposable3>());
                        }
                    }
                }
            }
        }

        [Test]
        public async Task Resolve_should_be_thread_safe()
        {
            using (var container = CreateContainer(x =>
            {
                x.RegisterSingleton<SlowDependency>(opt => opt.As<SlowDependency>().As<object>());
            }))
            {
                using (var scenario = container.BeginScope())
                {
                    var all = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Task.Run(() => scenario.Resolve<SlowDependency>()))
                        .Concat(Enumerable.Range(0, 10).Select(_ => Task.Run(() => container.Resolve<SlowDependency>()))));
                    Assert.AreEqual(20, all.Length);
                    Assert.AreEqual(1, all.Distinct().Count());
                    Assert.AreEqual(1, SlowDependency.Instances);
                }
            }
        }

        protected override IDependencyContainer CreateContainer()
        {
            return CreateContainer(x => x.RegisterSingleton(new DisposableSingleton()));
        }

        private static IDependencyContainer CreateContainer(Action<IDefaultContainerConfigurator> configurator)
        {
            return new DependencyContainerConfiguration()
                .UseDefault(configurator)
                .DependencyContainer;
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

        class Struct
        {
            public Struct(Disposable disposable)
            {
                Disposable = disposable;
            }

            public Disposable Disposable { get; }
        }

        class Complex
        {
            public Disposable Disposable { get; }
            public OtherComplex OtherComplex { get; }
            public Struct Value { get; }

            public Complex(Disposable disposable, OtherComplex otherComplex, Struct value)
            {
                Disposable = disposable;
                OtherComplex = otherComplex;
                Value = value;
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

        class FaultyDisposable : Disposable
        {
            public override void Dispose()
            {
                base.Dispose();
                throw new InvalidOperationException("boom!");
            }
        }
    }
}
