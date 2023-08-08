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
            return new DependencyContainerConfiguration().UseAutofac(builder).Build();
        }

        [Test]
        public void Resolve_should_honor_scopes()
        {
            using (var container = CreateContainer(x =>
            {
                x.RegisterType<Disposable>().SingleInstance();
                x.RegisterType<Disposable1>().InstancePerLifetimeScope();
                x.RegisterType<Disposable2>().InstancePerDependency();
            }))
            {
                Assert.AreSame(container.Resolve<Disposable1>(), container.Resolve<Disposable1>());
                using (var scenario = container.BeginScope())
                {
                    Assert.AreSame(container.Resolve<Disposable>(), scenario.Resolve<Disposable>());
                    Assert.AreSame(scenario.Resolve<Disposable1>(), scenario.Resolve<Disposable1>());
                    Assert.AreNotSame(container.Resolve<Disposable2>(), scenario.Resolve<Disposable2>());

                    using (var stepA = scenario.BeginScope())
                    {
                        Assert.AreSame(container.Resolve<Disposable>(), stepA.Resolve<Disposable>());
                        Assert.AreNotSame(scenario.Resolve<Disposable1>(), stepA.Resolve<Disposable1>());
                        Assert.AreSame(stepA.Resolve<Disposable1>(), stepA.Resolve<Disposable1>());
                        Assert.AreNotSame(stepA.Resolve<Disposable2>(), stepA.Resolve<Disposable2>());

                        using (var stepB = stepA.BeginScope())
                        {
                            Assert.AreSame(container.Resolve<Disposable>(), stepB.Resolve<Disposable>());
                            Assert.AreNotSame(stepA.Resolve<Disposable1>(), stepB.Resolve<Disposable1>());
                            Assert.AreSame(stepB.Resolve<Disposable1>(), stepB.Resolve<Disposable1>());
                            Assert.AreNotSame(stepB.Resolve<Disposable2>(), stepB.Resolve<Disposable2>());
                        }
                    }
                }
            }
        }
    }
}