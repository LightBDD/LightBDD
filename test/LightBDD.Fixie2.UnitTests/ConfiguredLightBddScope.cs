using System.Collections.Concurrent;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;

namespace LightBDD.Fixie2.UnitTests
{
    public class WithLightBddConventions : LightBddDiscoveryConvention { }

    public class ConfiguredLightBddScope : LightBddScope
    {
        public static readonly ConcurrentQueue<string> CapturedNotifications = new ConcurrentQueue<string>();
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ProgressNotifierConfiguration().Append(new DefaultProgressNotifier(x => CapturedNotifications.Enqueue(x)));
        }
    }
}