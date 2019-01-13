using System;
using LightBDD.Core.Notification;

namespace LightBDD.Framework.Notification.Implementation
{
    internal class StatefulScenarioProgressNotifierProvider<TFixture> : IScenarioProgressNotifierProvider
    {
        private readonly Func<TFixture, IScenarioProgressNotifier> _notifierProvider;

        public StatefulScenarioProgressNotifierProvider(Func<TFixture, IScenarioProgressNotifier> notifierProvider)
        {
            _notifierProvider = notifierProvider;
        }

        public IScenarioProgressNotifier Provide(object fixture)
        {
            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));

            if (fixture is TFixture stronglyTypedFixture)
                return _notifierProvider.Invoke(stronglyTypedFixture);

            throw new InvalidOperationException($"Unable to create {nameof(IScenarioProgressNotifier)}. Expected fixture of type '{typeof(TFixture)}' while got '{fixture.GetType()}'.");
        }
    }
}