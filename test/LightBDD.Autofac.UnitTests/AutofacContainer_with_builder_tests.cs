using System;
using Autofac;
using Autofac.Features.ResolveAnything;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Autofac.UnitTests
{
    [TestFixture]
    public class AutofacContainer_with_builder_tests : ContainerBaseTests
    {
        protected override IDependencyContainer CreateContainer()
        {
            return CreateContainer(builder => builder.Register(_ => new DisposableSingleton()).SingleInstance());
        }

        private static IDependencyContainer CreateContainer(Action<ContainerBuilder> configurator)
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            configurator(builder);
            return new DependencyContainerConfiguration().UseAutofac(builder).DependencyContainer;
        }

        [Test]
        public void Resolve_should_honor_scopes()
        {
            using (var container = CreateContainer(x =>
            {
                x.RegisterType<Disposable>().SingleInstance();
                x.RegisterType<Disposable1>().InstancePerMatchingLifetimeScope(LifetimeScope.Scenario);
                x.RegisterType<Disposable2>().InstancePerLifetimeScope();
                x.RegisterType<Disposable3>().InstancePerDependency();
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
    }
}