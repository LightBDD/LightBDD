using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;

namespace LightBDD.TUnit.UnitTests;

public class LightBddScopeAttribute_tests
{
    [Test]
    public async Task OnConfigure_failure_should_be_captured()
    {
        var scope = new TestableLightBddScopeAttribute();

        typeof(LightBddScopeAttribute).GetMethod("Configure", BindingFlags.Instance | BindingFlags.NonPublic)
            !.Invoke(scope, null);

        var exception = scope.Captured.ExecutionExtensionsConfiguration().FrameworkInitializationExceptions.FirstOrDefault();
        await Assert.That(exception).IsTypeOf<InvalidOperationException>();
        await Assert.That(exception!.Message).IsEqualTo("I failed!");
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