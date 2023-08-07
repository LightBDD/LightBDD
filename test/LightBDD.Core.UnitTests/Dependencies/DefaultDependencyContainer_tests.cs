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
        public void RegisterInstance_should_not_allow_to_register_as_invalid_type()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => container.BeginScope(LifetimeScope.Local, c => c.RegisterInstance(new Disposable(), new RegistrationOptions().As<string>())));
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
        public void It_should_resolve_complex_types_with_parameterized_ctors_honoring_singletons_registered_across_scopes()
        {
            var outerDisposable = new Disposable();
            var innerDisposable = new Disposable();
            var otherComplex = new OtherComplex(new Disposable());
            using (var container = CreateContainer())
            {
                using (var outerScope = container.BeginScope(LifetimeScope.Local, c =>
                {
                    c.RegisterInstance(outerDisposable, new RegistrationOptions());
                    c.RegisterInstance(otherComplex, new RegistrationOptions());
                }))
                {
                    using (var innerScope = outerScope.BeginScope(LifetimeScope.Local,
                               c => c.RegisterInstance(innerDisposable, new RegistrationOptions())))
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
                x.RegisterType<Disposable>(InstanceScope.Single);
                x.RegisterType<Disposable1>(InstanceScope.Scenario);
                x.RegisterType<Disposable2>(InstanceScope.Local);
                x.RegisterType<Disposable3>(InstanceScope.Transient);
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
                x.RegisterType<SlowDependency>(InstanceScope.Single, opt => opt.As<SlowDependency>().As<object>());
            }))
            {
                using (var scenario = container.BeginScope(LifetimeScope.Local))
                {
                    var all = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Task.Run(() => scenario.Resolve<SlowDependency>()))
                        .Concat(Enumerable.Range(0, 10).Select(_ => Task.Run(() => container.Resolve<SlowDependency>()))));
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
            Disposable3 d3;

            using (var container = CreateContainer(x =>
            {
                x.RegisterType<Disposable>(InstanceScope.Single);
                x.RegisterType(InstanceScope.Single, _ => new Disposable1());
                x.RegisterInstance(new Disposable2());
                x.RegisterInstance(new Disposable3(), opt => opt.ExternallyOwned());
            }))
            {
                d = container.Resolve<Disposable>();
                d1 = container.Resolve<Disposable1>();
                d2 = container.Resolve<Disposable2>();
                d3 = container.Resolve<Disposable3>();

                using (var scope = container.BeginScope(LifetimeScope.Local))
                {
                    Assert.AreSame(d, scope.Resolve<Disposable>());
                    Assert.AreSame(d1, scope.Resolve<Disposable1>());
                    Assert.AreSame(d2, scope.Resolve<Disposable2>());
                    Assert.AreSame(d3, scope.Resolve<Disposable3>());
                }
            }

            Assert.True(d.Disposed);
            Assert.True(d1.Disposed);
            Assert.True(d2.Disposed);
            Assert.False(d3.Disposed);
        }

        [Test]
        public void Register_scenario_instances_should_properly_honor_scopes()
        {
            Disposable d;
            Disposable1 d1;
            Disposable2 d2;

            using (var container = CreateContainer(x =>
            {
                x.RegisterType<Disposable>(InstanceScope.Scenario);
                x.RegisterType(InstanceScope.Scenario, _ => new Disposable1());
                x.RegisterType(InstanceScope.Scenario, _ => new Disposable2(), opt => opt.ExternallyOwned());
            }))
            {
                using (var scenarioScope = container.BeginScope(LifetimeScope.Scenario))
                {
                    d = scenarioScope.Resolve<Disposable>();
                    d1 = scenarioScope.Resolve<Disposable1>();
                    d2 = scenarioScope.Resolve<Disposable2>();

                    using (var scope2 = scenarioScope.BeginScope(LifetimeScope.Local))
                    {
                        Assert.AreSame(d, scope2.Resolve<Disposable>());
                        Assert.AreSame(d1, scope2.Resolve<Disposable1>());
                        Assert.AreSame(d2, scope2.Resolve<Disposable2>());
                    }
                }
            }

            Assert.True(d.Disposed);
            Assert.True(d1.Disposed);
            Assert.False(d2.Disposed);
        }

        [Test]
        public void Register_local_instances_should_properly_honor_scopes()
        {
            Disposable d;
            Disposable1 d1;
            Disposable2 d2;

            using (var container = CreateContainer(x =>
            {
                x.RegisterType<Disposable>(InstanceScope.Local);
                x.RegisterType(InstanceScope.Local, _ => new Disposable1());
                x.RegisterType(InstanceScope.Local, _ => new Disposable2(), opt => opt.ExternallyOwned());
            }))
            {
                d = container.Resolve<Disposable>();
                d1 = container.Resolve<Disposable1>();
                d2 = container.Resolve<Disposable2>();

                Assert.AreSame(d, container.Resolve<Disposable>());
                Assert.AreSame(d1, container.Resolve<Disposable1>());
                Assert.AreSame(d2, container.Resolve<Disposable2>());

                using (var scope = container.BeginScope(LifetimeScope.Local))
                {
                    Assert.AreNotSame(d, scope.Resolve<Disposable>());
                    Assert.AreNotSame(d1, scope.Resolve<Disposable1>());
                    Assert.AreNotSame(d2, scope.Resolve<Disposable2>());
                }
            }

            Assert.True(d.Disposed);
            Assert.True(d1.Disposed);
            Assert.False(d2.Disposed);
        }

        [Test]
        public void Register_transient_instances_should_properly_honor_scopes()
        {
            var resolved = new List<IDisposable>();
            using (var container = CreateContainer(x =>
            {
                x.RegisterType<Disposable>(InstanceScope.Transient);
                x.RegisterType(InstanceScope.Transient, _ => new Disposable1());
                x.RegisterType(InstanceScope.Transient, _ => new Disposable2(), opt => opt.ExternallyOwned());
                x.RegisterType<Disposable3>(InstanceScope.Transient, opt => opt.ExternallyOwned());
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

                using (var scope = container.BeginScope(LifetimeScope.Local))
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
            Assert.IsTrue(resolved.OfType<Disposable2>().All(d => !d.Disposed));
        }

        [Test]
        public void Container_should_honor_transient_fallback_resolution_behavior()
        {
            using (var container = CreateContainer(opt => opt.ConfigureFallbackBehavior(FallbackResolveBehavior.ResolveTransient)))
            {
                Assert.AreNotEqual(container.Resolve<Disposable>(), container.Resolve<Disposable>());
                using (var inner = container.BeginScope(LifetimeScope.Local))
                    Assert.AreNotEqual(container.Resolve<Disposable>(), inner.Resolve<Disposable>());
            }
        }


        [Test]
        public void Container_should_honor_throw_fallback_resolution_behavior()
        {
            using (var container = CreateContainer(opt => opt.ConfigureFallbackBehavior(FallbackResolveBehavior.ThrowException)))
            {
                Assert.Throws<InvalidOperationException>(() => container.Resolve<Disposable>());
                using (var inner = container.BeginScope(LifetimeScope.Local))
                    Assert.Throws<InvalidOperationException>(() => inner.Resolve<Disposable>());
            }
        }

        [Test]
        public void Container_should_provide_context_for_registered_instances_when_resolution_fails_for_throw_fallback()
        {
            using (var container = CreateContainer(x =>
            {
                x.ConfigureFallbackBehavior(FallbackResolveBehavior.ThrowException);
                x.RegisterInstance(new Disposable(), opt => opt.As<Disposable>().As<object>());
                x.RegisterType<Disposable1>(InstanceScope.Transient);
                x.RegisterType<Disposable2>(InstanceScope.Scenario, opt => opt.As<Disposable2>().As<IDisposable>());
                x.RegisterType<Disposable3>(InstanceScope.Local);
            }))
            {
                using (var scenario = container.BeginScope(LifetimeScope.Scenario))
                using (var step = scenario.BeginScope(LifetimeScope.Local))
                {
                    var ex = Assert.Throws<InvalidOperationException>(() => step.Resolve<Disposable4>());

                    Assert.AreEqual($@"Unable to resolve type {typeof(Disposable4)} from scope {LifetimeScope.Local}:
No suitable registration has been found to resolve type {typeof(Disposable4)}.
Available registrations:

Container scope: #local
{typeof(Disposable1)} -> #1 {typeof(Disposable1)} (Transient)
{typeof(Disposable3)} -> #2 {typeof(Disposable3)} (Local)

Container scope: #scenario
{typeof(Disposable1)} -> #1 {typeof(Disposable1)} (Transient)
{typeof(Disposable2)} -> #2 {typeof(Disposable2)} (Scenario)
{typeof(Disposable3)} -> #3 {typeof(Disposable3)} (Local)
{typeof(IDisposable)} -> #2 {typeof(Disposable2)} (Scenario)

Container scope: #global
{typeof(Disposable)} -> #1 {typeof(Disposable)} (Single)
{typeof(Disposable1)} -> #2 {typeof(Disposable1)} (Transient)
{typeof(Disposable3)} -> #3 {typeof(Disposable3)} (Local)
{typeof(object)} -> #1 {typeof(Disposable)} (Single)".NormalizeNewLine(), ex.Message.NormalizeNewLine());
                }
            }
        }

        [Test]
        public void Resolve_failure_should_provide_details_of_the_issue()
        {
            using (var container = CreateContainer(x =>
            {
                x.ConfigureFallbackBehavior(FallbackResolveBehavior.ThrowException);
                x.RegisterType<Struct>(InstanceScope.Single);
                x.RegisterType<OtherComplex>(InstanceScope.Scenario);
                x.RegisterType<Disposable>(InstanceScope.Scenario);
                x.RegisterType<Complex>(InstanceScope.Transient);
            }))
            {
                using (var scenario = container.BeginScope(LifetimeScope.Scenario))
                using (var step = scenario.BeginScope(LifetimeScope.Local))
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
            return CreateContainer(x => x.RegisterInstance(new DisposableSingleton()));
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
