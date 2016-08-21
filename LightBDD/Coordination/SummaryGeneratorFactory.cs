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
            return new SummaryGenerator(cfg.SummaryWriters.OfType<SummaryWriterElement>().Select(swe => CreateSummaryOutput(cfg.SummaryWriters.Type, swe)).ToArray());
        }

        private static ISummaryWriter CreateSummaryOutput(Type writerType, SummaryWriterElement summaryWriterElement)
        {
            var formatter = (IResultFormatter)Activator.CreateInstance(summaryWriterElement.Formatter);
            var param = new object[] { formatter, summaryWriterElement.Output };
            return (ISummaryWriter)Activator.CreateInstance(writerType, param);
        }
    }
}