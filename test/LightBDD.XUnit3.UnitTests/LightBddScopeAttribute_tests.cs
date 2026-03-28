using System;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using Xunit;

namespace LightBDD.XUnit3.UnitTests;

public class LightBddScopeAttribute_tests
{
    [Fact]
    public void OnConfigure_failure_should_be_captured()
    {
        var scope = new FailingScope();

        typeof(LightBddScope).GetMethod("Configure", BindingFlags.Instance | BindingFlags.NonPublic)
            !.Invoke(scope, null);

        var exception = scope.Captured.ExecutionExtensionsConfiguration().FrameworkInitializationExceptions.FirstOrDefault();
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("I failed!", exception.Message);
    }

    private class FailingScope : LightBddScope
    {
        public LightBddConfiguration Captured { get; private set; }
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            Captured = configuration;
            throw new InvalidOperationException("I failed!");
        }
    }
}
