using System;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using Shouldly;

namespace LightBDD.Fixie3.UnitTests;

public class LightBddScope_tests : FeatureFixture
{
    [Scenario]
    public void OnConfigure_failure_should_be_captured()
    {
        var scope = new ConfiguredLightBddScope() { ThrowOnConfigure = true };

        typeof(LightBddScope).GetMethod("Configure", BindingFlags.Instance | BindingFlags.NonPublic)
            !.Invoke(scope, null);

        var exception = scope.Captured.ExecutionExtensionsConfiguration().FrameworkInitializationExceptions.FirstOrDefault();
        exception.ShouldBeOfType<InvalidOperationException>().Message.ShouldBe("I failed!");
    }
}