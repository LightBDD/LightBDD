using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification.Configuration.Implementation;

namespace LightBDD.Framework.Notification.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize scenario progress notification behavior.
    /// </summary>
    [DebuggerStepThrough]
    public class ScenarioProgressNotifierConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns function providing scenario progress notifier, where function parameter is feature fixture class instance.
        /// By default it is initialized with function returning <see cref="NoProgressNotifier.Default"/> instance.
        /// </summary>
        public Func<object, IScenarioProgressNotifier> NotifierProvider { get; private set; } = fixture => NoProgressNotifier.Default;

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
            NotifierProvider = new StatelessScenarioProgressNotifierProvider(notifierProvider).Provide;
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
            NotifierProvider = new StatefulScenarioProgressNotifierProvider<TFixture>(notifierProvider).Provide;
            return this;
        }
    }
}