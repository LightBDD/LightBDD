using LightBDD.Fixie3.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;
using Shouldly;

namespace LightBDD.Fixie3.UnitTests;

public class AppendSimpleIndentedNotifier_tests
{
    public void AppendSimpleIndentedNotifier_should_return_same_configuration_for_chaining()
    {
        var cfg = new ProgressNotifierConfiguration();
        var result = cfg.AppendSimpleIndentedNotifier();
        result.ShouldBeSameAs(cfg);
    }

    public void AppendSimpleIndentedNotifier_should_replace_default_notifier()
    {
        var cfg = new ProgressNotifierConfiguration();
        cfg.AppendSimpleIndentedNotifier();
        cfg.Notifier.ShouldNotBeSameAs(NoProgressNotifier.Default);
    }
}
