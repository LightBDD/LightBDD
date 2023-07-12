using System.Collections.Generic;

namespace LightBDD.Core.Metadata;

/// <summary>
/// Test Run information.
/// </summary>
public interface ITestRunInfo : IMetadataInfo
{
    /// <summary>
    /// Test suite
    /// </summary>
    public TestSuite TestSuite { get; }
    /// <summary>
    /// List of LightBDD assemblies participating in tests
    /// </summary>
    public IReadOnlyList<AssemblyInfo> LightBddAssemblies { get; }
}