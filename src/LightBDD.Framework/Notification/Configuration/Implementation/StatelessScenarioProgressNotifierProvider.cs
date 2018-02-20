using System;
using System.Diagnostics;
using LightBDD.Core.Notification;

namespace LightBDD.Framework.Notification.Configuration.Implementation
{
    [DebuggerStepThrough]
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