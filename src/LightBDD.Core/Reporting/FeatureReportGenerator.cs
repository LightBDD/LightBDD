using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Results;

namespace LightBDD.Core.Reporting;

//TODO: LightBDD 4.x review testing strategy / internal access
/// <summary>
/// Feature report generator
/// </summary>
public class FeatureReportGenerator
{
    private readonly IReportGenerator[] _generators;

    /// <summary>
    /// Default constructor
    /// </summary>
    public FeatureReportGenerator(LightBddConfiguration configuration)
    {
        _generators = configuration.ReportConfiguration().Cast<IReportGenerator>().ToArray();
    }

    /// <summary>
    /// Generate reports for provided <paramref name="result"/>
    /// </summary>
    /// <param name="result">Test run result</param>
    /// <exception cref="AggregateException">Thrown if any report generator failed</exception>
    public async Task GenerateReports(ITestRunResult result)
    {
        var exceptions = new List<Exception>();

        foreach (var reportGenerator in _generators)
        {
            try
            {
                await reportGenerator.Generate(result);
            }
            catch (Exception ex) { exceptions.Add(ex); }
        }

        if (exceptions.Count > 0)
            throw new AggregateException("Failed to generate reports", exceptions);
    }
}