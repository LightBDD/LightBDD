using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification.Configuration.Implementation;

namespace LightBDD.Framework.Notification.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize scenario progress notification behavior.
    /// </summary>
    public class ScenarioProgressNotifierConfiguration : FeatureConfiguration
    {
        private readonly ScenarioProgressNotifierComposer _composer = new ScenarioProgressNotifierComposer();

        /// <summary>
        /// Returns function providing scenario progress notifier, where function parameter is feature fixture class instance.
        /// By default it is initialized to return <see cref="NoProgressNotifier.Default"/> instance.
        /// </summary>
        public Func<object, IScenarioProgressNotifier> NotifierProvider => _composer.Clone().Compose;

        /// <summary>
        /// Replaces the <see cref="NotifierProvider"/> with <paramref name="notifierProvider"/> value.
        /// </summary>
        /// <param name="notifierProvider">New provider to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifierProvider"/> is null.</exception>
        public ScenarioProgressNotifierConfiguration UpdateNotifierProvider(Func<IScenarioProgressNotifier> notifierProvider)
        {
            ThrowIfSealed();
            if (notifierProvider == null)
                throw new ArgumentNullException(nameof(notifierProvider));
            _composer.Set(new StatelessScenarioProgressNotifierProvider(notifierProvider));
            return this;
        }

        /// <summary>
        /// Replaces the <see cref="NotifierProvider"/> with <paramref name="notifierProvider"/> value.
        /// </summary>
        /// <param name="notifierProvider">New provider to set.</param>
        /// <typeparam name="TFixture">Feature fixture type.</typeparam>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifierProvider"/> is null.</exception>
        public ScenarioProgressNotifierConfiguration UpdateNotifierProvider<TFixture>(Func<TFixture, IScenarioProgressNotifier> notifierProvider)
        {
            ThrowIfSealed();
            if (notifierProvider == null)
                throw new ArgumentNullException(nameof(notifierProvider));
            _composer.Set(new StatefulScenarioProgressNotifierProvider<TFixture>(notifierProvider));
            return this;
        }

        /// <summary>
        /// Appends <paramref name="notifierProviders"/> to existing <see cref="NotifierProvider"/> making all of them used during notification.
        /// </summary>
        /// <param name="notifierProviders">Notifiers to append</param>
        /// <returns>Self</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifierProviders"/> collection or any of it's item is null.</exception>
        public ScenarioProgressNotifierConfiguration AppendNotifierProviders(params Func<IScenarioProgressNotifier>[] notifierProviders)
        {
            ThrowIfSealed();
            if (notifierProviders == null)
                throw new ArgumentNullException(nameof(notifierProviders));

            foreach (var notifierProvider in notifierProviders)
                _composer.Append(new StatelessScenarioProgressNotifierProvider(notifierProvider));
            return this;
        }

        /// <summary>
        /// Appends <paramref name="notifierProviders"/> to existing <see cref="NotifierProvider"/> making all of them used during notification.
        /// </summary>
        /// <param name="notifierProviders">Notifiers to append</param>
        /// <returns>Self</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifierProviders"/> collection or any of it's item is null.</exception>
        public ScenarioProgressNotifierConfiguration AppendNotifierProviders<TFixture>(params Func<TFixture, IScenarioProgressNotifier>[] notifierProviders)
        {
            ThrowIfSealed();
            if (notifierProviders == null)
                throw new ArgumentNullException(nameof(notifierProviders));

            foreach (var notifierProvider in notifierProviders)
                _composer.Append(new StatefulScenarioProgressNotifierProvider<TFixture>(notifierProvider));
            return this;
        }

        /// <summary>
        /// Clears <see cref="NotifierProvider"/> to use <see cref="NoProgressNotifier.Default"/> instance that does not report any notifications.
        /// </summary>
        /// <returns>Self.</returns>
        public ScenarioProgressNotifierConfiguration ClearNotifierProviders()
        {
            ThrowIfSealed();
            _composer.Clear();
            return this;
        }
    }
}