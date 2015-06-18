using System;
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
            var cfg = LightBDDConfiguration.GetConfiguration();
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