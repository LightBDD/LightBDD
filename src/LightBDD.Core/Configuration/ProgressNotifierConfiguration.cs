#nullable enable
using System;
using System.Collections.Generic;
using LightBDD.Core.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize scenario progress notification behavior.
    /// </summary>
    //TODO: consider generalizing collection registrations
    public class ProgressNotifierConfiguration : FeatureConfiguration
    {
        private readonly List<ServiceDescriptor> _notifiers = new();

        /// <summary>
        /// Returns <see cref="IProgressNotifier"/> registrations.<br/>
        /// By default it is configured with no notifiers.
        /// </summary>
        //TODO: review how these descriptors are registered. Consider hiding it. Consider also reworking global service collection to provide only one way to setup and clear specific registrations.
        public IReadOnlyList<ServiceDescriptor> Notifiers => _notifiers;

        /// <summary>
        /// Registers <see cref="IProgressNotifier"/> described by <paramref name="configurer"/> to notifier collection, making all of them used during notification.
        /// </summary>
        /// <param name="configurer"><see cref="IProgressNotifier"/> configurer</param>
        /// <returns>Self</returns>
        /// <exception cref="InvalidOperationException">Throws when configuration is sealed.</exception>
        public ProgressNotifierConfiguration Register(Action<FeatureConfigurer<IProgressNotifier>> configurer)
        {
            ThrowIfSealed();
            var cfg = new FeatureConfigurer<IProgressNotifier>();
            configurer.Invoke(cfg);
            _notifiers.Add(cfg.GetDescriptor());
            return this;
        }

        /// <summary>
        /// Clears <see cref="Notifiers"/>.
        /// </summary>
        /// <returns>Self.</returns>
        public ProgressNotifierConfiguration Clear()
        {
            ThrowIfSealed();
            _notifiers.Clear();
            return this;
        }
    }
}