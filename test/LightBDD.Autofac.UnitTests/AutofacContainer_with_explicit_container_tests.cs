using Autofac;
using Autofac.Features.ResolveAnything;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Autofac.UnitTests
{
    [TestFixture(true)]
    [TestFixture(false)]
    public class AutofacContainer_with_explicit_container_tests : ContainerBaseTests
    {
        public AutofacContainer_with_explicit_container_tests(bool shouldTakeOwnership)
            : base(shouldTakeOwnership)
        {
        }

        protected override IDependencyContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            builder.Register(_ => new DisposableSingleton()).SingleInstance();

            return new DependencyContainerConfiguration()
                .UseAutofac(builder.Build(), ShouldTakeOwnership)
                .Build();
        }
    }
}