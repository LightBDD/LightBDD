using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.XUnit2.Configuration;
using Xunit;

namespace LightBDD.XUnit2.UnitTests;

public class AppendSimpleIndentedNotifier_tests
{
    [Fact]
    public void AppendSimpleIndentedNotifier_should_return_same_configuration_for_chaining()
    {
        var cfg = new ProgressNotifierConfiguration();
        var result = cfg.AppendSimpleIndentedNotifier();
        Assert.Same(cfg, result);
    }

    [Fact]
    public void AppendSimpleIndentedNotifier_should_replace_default_notifier()
    {
        var cfg = new ProgressNotifierConfiguration();
        cfg.AppendSimpleIndentedNotifier();
        Assert.NotSame(NoProgressNotifier.Default, cfg.Notifier);
    }
}
