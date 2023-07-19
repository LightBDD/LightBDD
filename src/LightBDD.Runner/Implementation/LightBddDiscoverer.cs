using System.Linq;
using System.Reflection;
using LightBDD.Core.Discovery;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation;

internal class LightBddDiscoverer : TestFrameworkDiscoverer
{
    private readonly ITestCollection _dummyCollection;
    private readonly ScenarioDiscoverer _discoverer = new();

    public LightBddDiscoverer(IAssemblyInfo assemblyInfo, ISourceInformationProvider sourceProvider,
        IMessageSink diagnosticMessageSink) : base(assemblyInfo, sourceProvider, diagnosticMessageSink)
    {
        _dummyCollection = DummyTestCollection.Create(new TestAssembly(assemblyInfo));
    }

    protected override ITestClass CreateTestClass(ITypeInfo @class)
    {
        return new TestClass(_dummyCollection, @class);
    }

    protected override bool FindTestsForType(ITestClass testClass, bool includeSourceInformation, IMessageBus messageBus, ITestFrameworkDiscoveryOptions discoveryOptions)
    {
        var typeInfo = ((ReflectionTypeInfo)testClass.Class).Type.GetTypeInfo();

        foreach (var scenario in _discoverer.DiscoverFor(typeInfo))
        {
            var arguments = scenario.ScenarioArguments;
            if (arguments.Length == 0)
                arguments = null;

            var testMethod = new TestMethod(testClass, new ReflectionMethodInfo(scenario.ScenarioMethod));
            ITestCase testCase = new LightBddTestCase(DiagnosticMessageSink, discoveryOptions, testMethod, arguments);

            if (includeSourceInformation)
                testCase.SourceInformation = SourceProvider.GetSourceInformation(testCase);

            if (!messageBus.QueueMessage(new TestCaseDiscoveryMessage(testCase)))
                return false;
        }

        return true;
    }
}