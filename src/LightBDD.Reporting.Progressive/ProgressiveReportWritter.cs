using System.IO;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;

namespace LightBDD.Reporting.Progressive
{
    public class ProgressiveReportWritter : IReportWriter
    {
        public JsonlProgressNotifier Notifier { get; }

        public ProgressiveReportWritter()
        {
            Notifier = new JsonlProgressNotifier(File.OpenWrite("output.jsonl"));
        }

        public void Save(params IFeatureResult[] results)
        {
            Notifier.Dispose();
        }
    }
}