using System.IO;
using System.Text;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;

namespace LightBDD.Reporting.Progressive
{
    internal class ProgressiveReportWriter : IReportWriter
    {
        private readonly StreamWriter _writer;
        public JsonlProgressNotifier Notifier { get; }

        public ProgressiveReportWriter()
        {
            _writer = new StreamWriter(File.Create("output.jsonl"), Encoding.UTF8);
            Notifier = new JsonlProgressNotifier(_writer.WriteLineAsync);
        }

        public void Save(params IFeatureResult[] results)
        {
            Notifier.Dispose();
            _writer.Dispose();
        }
    }
}