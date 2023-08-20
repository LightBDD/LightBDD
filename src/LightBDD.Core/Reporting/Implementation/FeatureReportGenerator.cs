using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;

namespace LightBDD.Core.Reporting.Implementation;

internal class FeatureReportGenerator
{
    private readonly IReportGenerator[] _generators;

    public FeatureReportGenerator(IEnumerable<IReportGenerator> generators)
    {
        _generators = generators.ToArray();
    }

    public async Task GenerateReports(ITestRunResult result)
    {
        var exceptions = new List<Exception>();

        foreach (var reportGenerator in _generators)
        {
            try
            {
                await reportGenerator.Generate(result);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
            throw new AggregateException("Failed to generate reports", exceptions);
    }
}