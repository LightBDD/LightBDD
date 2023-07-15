using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddTest : ITest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XunitTest"/> class.
    /// </summary>
    /// <param name="testCase">The test case this test belongs to.</param>
    /// <param name="displayName">The display name for this test.</param>
    public LightBddTest(ITestCase testCase, string displayName)
    {
        TestCase = testCase;
        DisplayName = displayName;
    }

    /// <inheritdoc/>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Gets the xUnit v2 test case.
    /// </summary>
    public ITestCase TestCase { get; private set; }
}