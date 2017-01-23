using LightBDD.Reporting.Formatters;

namespace LightBDD.Reporting.Configuration
{
    public static class SummaryFileWriterConfigurationExtensions
    {
        public static SummaryWritersConfiguration AddFileWriter<TFormatter>(this SummaryWritersConfiguration cfg, string outputPath) where TFormatter : IResultFormatter, new()
        {
            return cfg.Add(new SummaryFileWriter(new TFormatter(), outputPath));
        }
    }
}