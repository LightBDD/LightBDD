using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.UnitTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Dependencies
{
    [TestFixture]
    [Ignore("to review")]
    public class DefaultDependencyContainer_tests : ContainerBaseTests
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
        [Ignore("To rewrite")]
        public void RegisterInstance_should_not_allow_to_register_as_invalid_type()
        {
            using (var container = CreateContainer())
            {
                //c => c.RegisterInstance(new Disposable(), new RegistrationOptions().As<string>())
                var ex = Assert.Throws<InvalidOperationException>(() => container.BeginScope());
                Assert.That(ex.Message, Is.EqualTo($"Type {typeof(Disposable)} is not assignable to {typeof(string)}"));
            }
        }

        [Test]
        public void It_should_throw_if_type_has_no_public_ctors()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<NoCtorType>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(NoCtorType)} from scope {LifetimeScope.Global}:{Environment.NewLine}Type '{typeof(NoCtorType)}' has to have exactly one public constructor (number of public constructors: 0)."));
            }
        }

        [Test]
        public void It_should_throw_if_type_is_interface_and_have_no_explicit_registrations()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<IDisposable>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(IDisposable)} from scope {LifetimeScope.Global}:{Environment.NewLine}Type '{typeof(IDisposable)}' has to be non-abstract class or value type."));
            }
        }

        [Test]
        public void It_should_throw_if_type_is_abstract_and_have_no_explicit_registrations()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<Abstract>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(Abstract)} from scope {LifetimeScope.Global}:{Environment.NewLine}Type '{typeof(Abstract)}' has to be non-abstract class or value type."));
            }
        }

        [Test]
        public void It_should_throw_if_type_has_multiple_public_ctors()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.Resolve<MultiCtorType>());
                Assert.That(ex.Message, Is.EqualTo($"Unable to resolve type {typeof(MultiCtorType)} from scope {LifetimeScope.Global}:{Environment.NewLine}Type '{typeof(MultiCtorType)}' has to have exactly one public constructor (number of public constructors: 2)."));
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
        [Ignore("To rewrite")]
        public void It_should_resolve_complex_types_with_parameterized_ctors_honoring_singletons_registered_across_scopes()
        {
            var outerDisposable = new Disposable();
            var innerDisposable = new Disposable();
            var otherComplex = new OtherComplex(new Disposable());
            using (var container = CreateContainer())
            {
                /*
                 c =>
                {
                    c.RegisterInstance(outerDisposable, new RegistrationOptions());
                    c.RegisterInstance(otherComplex, new RegistrationOptions());
                }
                 */
                using (var outerScope = container.BeginScope())
                {
                    //c => c.RegisterInstance(innerDisposable, new RegistrationOptions())
                    using (var innerScope = outerScope.BeginScope())
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

                Assert.That(ex.Message.NormalizeNewLine(), Is.EqualTo($@"Unable to resolve type {typeof(Holder<ProblematicType>)} from scope #global:
Unable to resolve type {typeof(ProblematicType)} from scope #global:
Unable to resolve type {typeof(MultiCtorType)} from scope #global:
Type '{typeof(MultiCtorType)}' has to have exactly one public constructor (number of public constructors: 2).".NormalizeNewLine()));
            }
        }

        [Test]
        public void Resolve_should_honor_scopes()
        {
            using (var container = CreateContainer(x =>
            {
                x.AddSingleton<Disposable>();
                x.AddScoped<Disposable1>();
                x.AddTransient<Disposable2>();
            }))
            {
                using (var scenario = container.BeginScope())
                {
                    Assert.AreSame(container.Resolve<Disposable>(), scenario.Resolve<Disposable>());
                    Assert.AreSame(scenario.Resolve<Disposable1>(), scenario.Resolve<Disposable1>());
                    Assert.AreNotSame(container.Resolve<Disposable1>(), scenario.Resolve<Disposable1>());
                    Assert.AreNotSame(scenario.Resolve<Disposable2>(), scenario.Resolve<Disposable2>());
                }
            }
        }

        [Test]
        public async Task Resolve_should_be_thread_safe()
        {
            using (var container = CreateContainer(x =>
            {
                x.AddSingleton<SlowDependency>();
                x.AddSingleton<object>(x=>x.GetRequiredService<SlowDependency>());
            }))
            {
                using (var scenario = container.BeginScope())
                {
                    var all = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Task.Run(() => (object)scenario.Resolve<SlowDependency>()))
                        .Concat(Enumerable.Range(0, 10).Select(_ => Task.Run(() => container.Resolve<object>()))));
                    Assert.AreEqual(20, all.Length);
                    Assert.AreEqual(1, all.Distinct().Count());
                    Assert.AreEqual(1, SlowDependency.Instances);
                }
            }
        }

        [Test]
        public void Register_single_instances_should_properly_honor_scopes()
        {
            Disposable d;
            Disposable1 d1;
            Disposable2 d2;

            using (var container = CreateContainer(x =>
            {
                x.AddSingleton<Disposable>();
                x.AddSingleton(_ => new Disposable1());
                x.AddSingleton(new Disposable2());
            }))
            {
                d = container.Resolve<Disposable>();
                d1 = container.Resolve<Disposable1>();
                d2 = container.Resolve<Disposable2>();

                using (var scope = container.BeginScope())
                {
                    Assert.AreSame(d, scope.Resolve<Disposable>());
                    Assert.AreSame(d1, scope.Resolve<Disposable1>());
                    Assert.AreSame(d2, scope.Resolve<Disposable2>());
                }
            }

            Assert.True(d.Disposed);
            Assert.True(d1.Disposed);
            Assert.True(d2.Disposed);
        }

        [Test]
        public void Register_scenario_instances_should_properly_honor_scopes()
        {
            Disposable1 s1d1,s2d1;
            Disposable2 s1d2,s2d2;

            using (var container = CreateContainer(x =>
            {
                x.AddScoped<Disposable1>();
                x.AddScoped(_ => new Disposable2());
            }))
            {
                using (var scope1 = container.BeginScope())
                using (var scope2 = container.BeginScope())
                {
                    s1d1 = scope1.Resolve<Disposable1>();
                    s1d2 = scope1.Resolve<Disposable2>();
                    s2d1 = scope2.Resolve<Disposable1>();
                    s2d2 = scope2.Resolve<Disposable2>();
                    Assert.AreNotSame(s1d1,s2d1);
                    Assert.AreNotSame(s1d2,s2d2);
                    Assert.AreSame(s1d1,scope1.Resolve<Disposable1>());
                    Assert.AreSame(s1d2,scope1.Resolve<Disposable2>());
                    Assert.AreSame(s2d1, scope2.Resolve<Disposable1>());
                    Assert.AreSame(s2d2, scope2.Resolve<Disposable2>());
                }
            }

            Assert.True(s1d1.Disposed);
            Assert.True(s1d2.Disposed);
            Assert.True(s2d1.Disposed);
            Assert.True(s2d2.Disposed);
        }

        [Test]
        public void Register_transient_instances_should_properly_honor_scopes()
        {
            var resolved = new List<IDisposable>();
            using (var container = CreateContainer(x =>
            {
                x.AddTransient<Disposable>();
                x.AddTransient(_ => new Disposable1());
            }))
            {
                var d = container.Resolve<Disposable>();
                var d1 = container.Resolve<Disposable1>();
                var d2 = container.Resolve<Disposable2>();
                resolved.Add(d);
                resolved.Add(d1);
                resolved.Add(d2);

                Assert.AreNotSame(d, container.Resolve<Disposable>());
                Assert.AreNotSame(d1, container.Resolve<Disposable1>());
                Assert.AreNotSame(d2, container.Resolve<Disposable2>());

                using (var scope = container.BeginScope())
                {
                    var sd = scope.Resolve<Disposable>();
                    var sd1 = scope.Resolve<Disposable1>();
                    var sd2 = scope.Resolve<Disposable2>();
                    Assert.AreNotSame(sd, scope.Resolve<Disposable>());
                    Assert.AreNotSame(sd1, scope.Resolve<Disposable1>());
                    Assert.AreNotSame(sd2, scope.Resolve<Disposable2>());
                }
            }

            Assert.IsTrue(resolved.OfType<Disposable>().All(d => d.Disposed));
            Assert.IsTrue(resolved.OfType<Disposable1>().All(d => d.Disposed));
            Assert.IsTrue(resolved.OfType<Disposable2>().All(d => d.Disposed));
        }

        [Test]
        public void Container_should_honor_transient_fallback_resolution_behavior()
        {
            using (var container = CreateContainer())
            {
                Assert.AreNotEqual(container.Resolve<Disposable>(), container.Resolve<Disposable>());
                using (var inner = container.BeginScope())
                    Assert.AreNotEqual(container.Resolve<Disposable>(), inner.Resolve<Disposable>());
            }
        }


        [Test]
        [Ignore("rewrite")]
        public void Container_should_honor_throw_fallback_resolution_behavior()
        {
            using (var container = CreateContainer(/*opt => opt.ConfigureFallbackBehavior(FallbackResolveBehavior.ThrowException)*/))
            {
                Assert.Throws<InvalidOperationException>(() => container.Resolve<Disposable>());
                using (var inner = container.BeginScope())
                    Assert.Throws<InvalidOperationException>(() => inner.Resolve<Disposable>());
            }
        }

        [Test]
        [Ignore("rewrite")]
        public void Resolve_failure_should_provide_details_of_the_issue()
        {
            using (var container = CreateContainer(x =>
            {
                //x.ConfigureFallbackBehavior(FallbackResolveBehavior.ThrowException);
                x.AddSingleton<Struct>();
                x.AddScoped<OtherComplex>();
                x.AddScoped<Disposable>();
                x.AddTransient<Complex>();
            }))
            {
                using (var scenario = container.BeginScope())
                using (var step = scenario.BeginScope())
                {
                    var ex = Assert.Throws<InvalidOperationException>(() => step.Resolve<Complex>());
                    Assert.That(ex.Message.NormalizeNewLine(), Is.EqualTo($@"Unable to resolve type {typeof(Complex)} from scope #local:
Unable to instantiate type {typeof(Complex)} in scope #local:
Unable to resolve type {typeof(Struct)} from scope #local:
Unable to instantiate type {typeof(Struct)} in scope #global:
Unable to resolve type {typeof(Disposable)} from scope #global:
No suitable registration has been found to resolve type {typeof(Disposable)}.
Available registrations:

Container scope: #global
{typeof(Complex)} -> #2 {typeof(Complex)} (Transient)
{typeof(Struct)} -> #1 {typeof(Struct)} (Single)".NormalizeNewLine()));
                }
            }
        }

        protected override IDependencyContainer CreateContainer()
        {
            return CreateContainer(x => x.AddSingleton(new DisposableSingleton()));
        }

        private static IDependencyContainer CreateContainer(Action<IServiceCollection> configurator)
        {
            return new DependencyContainerConfiguration()
                .ConfigureServices(configurator)
                .Build();
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
