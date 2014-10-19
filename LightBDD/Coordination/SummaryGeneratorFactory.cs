using System;
using System.Configuration;
using System.Linq;
using LightBDD.Configuration;
using LightBDD.Results.Formatters;
using LightBDD.SummaryGeneration;

namespace LightBDD.Coordination
{
    internal class SummaryGeneratorFactory
    {
        public static SummaryGenerator Create()
        {
            var cfg = ConfigurationManager.GetSection("lightbdd") as LightBDDConfiguration ?? new LightBDDConfiguration();
            return new SummaryGenerator(cfg.SummaryWriters.OfType<SummaryWriterElement>().Select(CreateSummaryOutput).ToArray());
        }

        private static SummaryFileWriter CreateSummaryOutput(SummaryWriterElement summaryWriterElement)
        {
            return new SummaryFileWriter(
                (IResultFormatter)Activator.CreateInstance(summaryWriterElement.Formatter),
                summaryWriterElement.Output);
        }
    }
}