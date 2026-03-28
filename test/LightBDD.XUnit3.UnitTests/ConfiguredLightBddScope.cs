using System.Collections.Concurrent;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.XUnit3;
using LightBDD.XUnit3.UnitTests;
using Xunit.v3;

[assembly: TestPipelineStartup(typeof(ConfiguredLightBddScope))]
namespace LightBDD.XUnit3.UnitTests
{
    public class ConfiguredLightBddScope : LightBddScope
    {
        public static readonly ConcurrentQueue<string> CapturedNotifications = new();

        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportWritersConfiguration()
                .Clear();

            configuration.ProgressNotifierConfiguration()
                .Clear()
                .Append(new DefaultProgressNotifier(CapturedNotifications.Enqueue));
        }
    }
}
