using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Results;

namespace LightBDD.Core.Reporting.Implementation;

internal class FeatureReportGeneratorV2
{
    private readonly IReportWriter[] _writers;

    public FeatureReportGeneratorV2(LightBddConfiguration configuration)
    {
        _writers = configuration.ReportWritersConfiguration().ToArray();
    }

    public async Task GenerateReports(ITestRunResult results)
    {
        //Unnecessary ordering
        var features = results.GetFeatures().OrderBy(r => r.Info.Name.ToString(), StringComparer.OrdinalIgnoreCase).ToArray();
        foreach (var writer in _writers)
            writer.Save(features);
    }
}