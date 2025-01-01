using System;
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
        public bool ThrowOnConfigure { get; set; }
        public LightBddConfiguration Captured { get; private set; }
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            Captured = configuration;

            configuration.ReportWritersConfiguration()
                .Clear();

            configuration.ProgressNotifierConfiguration()
                .Clear()
                .Append(new DefaultProgressNotifier(CapturedNotifications.Enqueue));

            if (ThrowOnConfigure)
                throw new InvalidOperationException("I failed!");
        }
    }
}