using System.Threading.Tasks;
using LightBDD.Core.Results;

namespace LightBDD.Core.Reporting;

/// <summary>
/// Interface for test run result report generation.
/// </summary>
public interface IReportGenerator
{
    /// <summary>
    /// Generates report for <paramref name="result"/>.
    /// </summary>
    /// <param name="result">Test run results.</param>
    Task Generate(ITestRunResult result);
}