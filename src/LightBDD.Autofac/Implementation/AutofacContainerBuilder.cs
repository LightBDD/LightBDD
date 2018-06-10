using System;
using System.Linq;
using Autofac;
using LightBDD.Core.Dependencies;

namespace LightBDD.Autofac.Implementation
{
    internal class AutofacContainerBuilder : ContainerConfigurator
    {
        private readonly ContainerBuilder _builder;

        public AutofacContainerBuilder(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void Configure(Action<ContainerConfigurator> configuration)
        {
            configuration?.Invoke(this);
        }

        public override void RegisterInstance(object instance, RegistrationOptions options)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var registration = _builder
                .RegisterInstance(instance)
                .As(options.AsTypes.ToArray());

            if (options.IsExternallyOwned)
                registration.ExternallyOwned();
        }
    }
}
