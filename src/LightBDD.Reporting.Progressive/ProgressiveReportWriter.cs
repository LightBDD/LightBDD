using System.IO;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;

namespace LightBDD.Reporting.Progressive
{
    internal class ProgressiveReportWriter : IReportWriter
    {
        public JsonlProgressNotifier Notifier { get; }

        public ProgressiveReportWriter()
        {
            Notifier = new JsonlProgressNotifier(File.OpenWrite("output.jsonl"));
        }

        public void Save(params IFeatureResult[] results)
        {
            Notifier.Dispose();
        }
    }
}