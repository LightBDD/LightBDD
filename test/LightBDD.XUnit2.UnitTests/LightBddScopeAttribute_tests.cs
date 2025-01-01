using System;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using Xunit;

namespace LightBDD.XUnit2.UnitTests;

public class LightBddScopeAttribute_tests
{
    [Fact]
    public void OnConfigure_failure_should_be_captured()
    {
        var scope = new ConfiguredLightBddScope { ThrowOnConfigure = true };

        typeof(LightBddScopeAttribute).GetMethod("Configure", BindingFlags.Instance | BindingFlags.NonPublic)
            !.Invoke(scope, null);

        var exception = scope.Captured.ExecutionExtensionsConfiguration().FrameworkInitializationExceptions.FirstOrDefault();
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("I failed!", exception.Message);
    }
}