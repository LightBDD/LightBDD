using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.MsTest4.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest4.UnitTests;

[TestClass]
public class AppendSimpleIndentedNotifier_tests
{
    [TestMethod]
    public void AppendSimpleIndentedNotifier_should_return_same_configuration_for_chaining()
    {
        var cfg = new ProgressNotifierConfiguration();
        var result = cfg.AppendSimpleIndentedNotifier();
        Assert.AreSame(cfg, result);
    }

    [TestMethod]
    public void AppendSimpleIndentedNotifier_should_replace_default_notifier()
    {
        var cfg = new ProgressNotifierConfiguration();
        cfg.AppendSimpleIndentedNotifier();
        Assert.AreNotSame(NoProgressNotifier.Default, cfg.Notifier);
    }
}
