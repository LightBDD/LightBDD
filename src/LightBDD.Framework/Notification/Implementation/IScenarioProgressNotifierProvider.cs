using LightBDD.Core.Notification;

namespace LightBDD.Framework.Notification.Implementation
{
    internal interface IScenarioProgressNotifierProvider
    {
        IScenarioProgressNotifier Provide(object fixture);
    }
}