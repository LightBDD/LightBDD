using Autofac;
using Autofac.Features.ResolveAnything;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Autofac.UnitTests
{
    [TestFixture]
    public class AutofacContainer_tests : ContainerBaseTests
    {
        protected override IDependencyContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            return new DependencyContainerConfiguration().UseAutofac(builder).DependencyContainer;
        }
    }

    [TestFixture]
    public class AutofacContainer_inner_scope_tests : ContainerBaseTests
    {
        protected override IDependencyContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            return new DependencyContainerConfiguration().UseAutofac(builder.Build()).DependencyContainer;
        }
    }
}
