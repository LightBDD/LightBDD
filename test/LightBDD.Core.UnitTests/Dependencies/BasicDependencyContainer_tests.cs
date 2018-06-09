using System;
using System.Collections.Generic;
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
        public void It_should_resolve_complex_types_with_parameterized_ctors_choosing_the_longer_constructor()
        {
            Complex complex;
            using (var container = CreateContainer())
            {
                complex = container.Resolve<Complex>();
                Assert.That(complex, Is.Not.Null);
                Assert.That(complex.Disposable, Is.Not.Null);
                Assert.That(complex.OtherComplex, Is.Not.Null);
                Assert.That(complex.OtherComplex.Disposable, Is.Not.Null);

                Assert.That(complex.Disposable, Is.Not.SameAs(complex.OtherComplex.Disposable));
            }
            Assert.That(complex.Disposable.Disposed, Is.True);
            Assert.That(complex.OtherComplex.Disposable.Disposed, Is.True);
        }

        [Test]
        public void It_should_resolve_complex_types_with_parameterized_ctors_honoring_sigletons_registered_across_scopes()
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

        protected override IDependencyContainer CreateContainer()
        {
            return new DependencyContainerConfiguration().UseDefaultContainer().DependencyContainer;
        }

        class Complex
        {
            public Disposable Disposable { get; }
            public OtherComplex OtherComplex { get; }

            public Complex(OtherComplex otherComplex) : this(null, otherComplex) { }
            public Complex(Disposable disposable, OtherComplex otherComplex)
            {
                Disposable = disposable;
                OtherComplex = otherComplex;
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
    }


}
