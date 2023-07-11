using System.Collections.Concurrent;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;

namespace LightBDD.Fixie3.UnitTests
{
    public class ConfiguredLightBddScope : LightBddScope
    {
        public static readonly ConcurrentQueue<string> CapturedNotifications = new();
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportConfiguration()
                .Clear();

            configuration.ProgressNotifierConfiguration()
                .Clear()
                .Append(new DefaultProgressNotifier(x => CapturedNotifications.Enqueue(x)));
        }
    }
}