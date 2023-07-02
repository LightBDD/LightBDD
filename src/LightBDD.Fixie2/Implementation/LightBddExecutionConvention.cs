using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fixie;

namespace LightBDD.Fixie3.Implementation;

internal class LightBddExecutionConvention : IExecution
{
    private readonly LightBddScope _scope;

    public LightBddExecutionConvention(LightBddScope scope)
    {
        _scope = scope;
    }

    public async Task Run(TestSuite testSuite)
    {
        _scope.SetUp();
        try
        {
            foreach (var testClass in testSuite.TestClasses)
                foreach (var test in testClass.Tests)
                    await RunTest(testClass, test);
        }
        finally
        {
            _scope.TearDown();
        }
    }

    private async Task RunTest(TestClass testClass, Test test)
    {
        foreach (var parameters in GetParameters(test))
            await RunCase(testClass, test, parameters);
    }

    private async Task RunCase(TestClass testClass, Test test, object[] parameters)
    {
        var instance = testClass.Construct();
        try
        {
            TestContextProvider.Initialize(test.Method, parameters);
            await test.Method.Call(instance, parameters);
            await test.Pass(parameters);
        }
        catch (IgnoreException ex)
        {
            await test.Skip(parameters, ex.Message);
        }
        catch (Exception ex)
        {
            await test.Fail(parameters, ex);
        }
        finally
        {
            TestContextProvider.Clear();
            (instance as IDisposable)?.Dispose();
        }
    }

    private IEnumerable<object[]> GetParameters(Test test)
    {
        if (!test.HasParameters)
            return Enumerable.Repeat(Array.Empty<object>(), 1);

        return test.Method.GetCustomAttributes().OfType<IScenarioCaseSourceAttribute>().SelectMany(s => s.GetCases());
    }
}