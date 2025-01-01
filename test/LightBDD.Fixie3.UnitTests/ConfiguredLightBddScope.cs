using System;
using System.Collections.Concurrent;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;

namespace LightBDD.Fixie3.UnitTests
{
    public class ConfiguredLightBddScope : LightBddScope
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
                .Append(new DefaultProgressNotifier(x => CapturedNotifications.Enqueue(x)));
            if (ThrowOnConfigure)
                throw new InvalidOperationException("I failed!");
        }
    }
}