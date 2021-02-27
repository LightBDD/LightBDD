using System;
using LightBDD.Core.Notification;

namespace LightBDD.Framework.Notification.Implementation
{
    [Obsolete]
    internal interface IScenarioProgressNotifierProvider
    {
        IScenarioProgressNotifier Provide(object fixture);
    }
}