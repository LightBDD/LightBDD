using System.Collections.Concurrent;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.XUnit2.UnitTests;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.XUnit2.UnitTests
{
    internal class ConfiguredLightBddScope : LightBddScopeAttribute
    {
        public static readonly ConcurrentQueue<string> CapturedNotifications = new();
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportWritersConfiguration()
                .Clear();

            configuration.ProgressNotifierConfiguration()
                .Clear()
                .Append(new DefaultProgressNotifier(x => CapturedNotifications.Enqueue(x)));
        }
    }
}