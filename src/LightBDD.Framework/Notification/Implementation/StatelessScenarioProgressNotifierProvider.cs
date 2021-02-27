using System;
using LightBDD.Core.Notification;

namespace LightBDD.Framework.Notification.Implementation
{
    [Obsolete]
    internal class StatelessScenarioProgressNotifierProvider : IScenarioProgressNotifierProvider
    {
        private readonly Func<IScenarioProgressNotifier> _notifierProvider;

        public StatelessScenarioProgressNotifierProvider(Func<IScenarioProgressNotifier> notifierProvider)
        {
            _notifierProvider = notifierProvider;
        }

        public IScenarioProgressNotifier Provide(object fixture)
        {
            return _notifierProvider.Invoke();
        }
    }
}