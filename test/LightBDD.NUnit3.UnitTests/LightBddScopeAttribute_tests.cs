using System;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests;

[TestFixture]
public class LightBddScopeAttribute_tests
{
    [Test]
    public void OnConfigure_failure_should_be_captured()
    {
        var scope = new TestableLightBddScopeAttribute();

        typeof(LightBddScopeAttribute).GetMethod("Configure", BindingFlags.Instance | BindingFlags.NonPublic)
            !.Invoke(scope, null);

        var exception = scope.Captured.ExecutionExtensionsConfiguration().FrameworkInitializationExceptions.FirstOrDefault();
        Assert.That(exception, Is.TypeOf<InvalidOperationException>());
        Assert.That(exception!.Message, Is.EqualTo("I failed!"));
    }

    class TestableLightBddScopeAttribute : LightBddScopeAttribute
    {
        public LightBddConfiguration Captured { get; private set; }

        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            Captured = configuration;
            throw new InvalidOperationException("I failed!");
        }
    }
}