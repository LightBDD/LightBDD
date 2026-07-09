using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.NUnit3.Configuration;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests;

[TestFixture]
public class AppendSimpleIndentedNotifier_tests
{
    [Test]
    public void AppendSimpleIndentedNotifier_should_return_same_configuration_for_chaining()
    {
        var cfg = new ProgressNotifierConfiguration();
        var result = cfg.AppendSimpleIndentedNotifier();
        Assert.That(result, Is.SameAs(cfg));
    }

    [Test]
    public void AppendSimpleIndentedNotifier_should_replace_default_notifier()
    {
        var cfg = new ProgressNotifierConfiguration();
        cfg.AppendSimpleIndentedNotifier();
        Assert.That(cfg.Notifier, Is.Not.SameAs(NoProgressNotifier.Default));
    }
}
