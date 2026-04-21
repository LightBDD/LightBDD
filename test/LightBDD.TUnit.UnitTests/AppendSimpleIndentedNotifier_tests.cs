using System.Threading.Tasks;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.TUnit.Configuration;

namespace LightBDD.TUnit.UnitTests;

public class AppendSimpleIndentedNotifier_tests
{
    [Test]
    public async Task AppendSimpleIndentedNotifier_should_return_same_configuration_for_chaining()
    {
        var cfg = new ProgressNotifierConfiguration();
        var result = cfg.AppendSimpleIndentedNotifier();
        await Assert.That(result).IsSameReferenceAs(cfg);
    }

    [Test]
    public async Task AppendSimpleIndentedNotifier_should_replace_default_notifier()
    {
        var cfg = new ProgressNotifierConfiguration();
        cfg.AppendSimpleIndentedNotifier();
        await Assert.That(cfg.Notifier).IsNotSameReferenceAs(NoProgressNotifier.Default);
    }
}
