﻿using System;
using Autofac;
using LightBDD.Core.Dependencies;

namespace LightBDD.Autofac.Implementation
{
    internal class AutofacContainerBuilder : IContainerConfigurer
    {
        public ContainerBuilder Builder { get; }

        public AutofacContainerBuilder(ContainerBuilder builder)
        {
            Builder = builder;
        }

        public void Configure(Action<IContainerConfigurer> configuration)
        {
            configuration?.Invoke(this);
        }

        void IContainerConfigurer.RegisterInstance(object instance, RegistrationOptions options)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var registration = Builder
                .RegisterInstance(instance)
                .As(instance.GetType());

            if (!options.TakeOwnership)
                registration.ExternallyOwned();
        }
    }
}