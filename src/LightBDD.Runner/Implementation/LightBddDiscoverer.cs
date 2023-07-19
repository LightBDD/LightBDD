using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddDiscoverer : TestFrameworkDiscoverer
{
    private readonly ITestCollection _dummyCollection;

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
        foreach (var method in testClass.Class.GetMethods(true))
        {
            if (!method.GetCustomAttributes(typeof(ScenarioAttribute)).Any()) continue;

            ITestCase testCase = new LightBddTestCase(DiagnosticMessageSink,discoveryOptions, new TestMethod(testClass, method));
            if (includeSourceInformation)
                testCase.SourceInformation = SourceProvider.GetSourceInformation(testCase);

            if (!messageBus.QueueMessage(new TestCaseDiscoveryMessage(testCase)))
                return false;
        }

        return true;
    }
}