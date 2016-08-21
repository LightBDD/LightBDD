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
            return new SummaryGenerator(cfg.SummaryWriters.OfType<SummaryWriterElement>().Select(swe => CreateSummaryOutput(cfg, swe)).ToArray());
        }

        private static ISummaryWriter CreateSummaryOutput(LightBDDConfiguration cfg, SummaryWriterElement summaryWriterElement)
        {
            Type typeFormatter = cfg.Writer.SummaryWriter ?? typeof(SummaryFileWriter);

            IResultFormatter formatter = (IResultFormatter)Activator.CreateInstance(summaryWriterElement.Formatter);

            object[] param = new object[] {formatter, summaryWriterElement.Output};

            return (ISummaryWriter)Activator.CreateInstance(typeFormatter, param);
        }
    }
}