using System;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest3.UnitTests;

[TestClass]
public class LightBddScopeAttribute_tests
{
    [TestMethod]
    public void OnConfigure_failure_should_be_captured()
    {
        LightBddConfiguration captured = null;
        Action<LightBddConfiguration> onConfigure = cfg =>
        {
            captured = cfg;
            throw new InvalidOperationException("I failed!");
        };
        
        typeof(LightBddScope).GetMethod("Configure", BindingFlags.Static | BindingFlags.NonPublic)
            !.Invoke(null, [onConfigure]);

        var exception = captured.ExecutionExtensionsConfiguration().FrameworkInitializationExceptions.FirstOrDefault();
        Assert.IsInstanceOfType<InvalidOperationException>(exception);
        Assert.AreEqual("I failed!", exception.Message);
    }
}