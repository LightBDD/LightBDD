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
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            builder.Register(_ => new DisposableSingleton()).SingleInstance();
            return new DependencyContainerConfiguration().UseAutofac(builder).DependencyContainer;
        }
    }
}