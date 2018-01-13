using LightBDD.Core.Notification;

namespace LightBDD.Framework.Notification.Configuration.Implementation
{
    internal interface IScenarioProgressNotifierProvider
    {
        IScenarioProgressNotifier Provide(object fixture);
    }
}