using LightBDD.SummaryGeneration.Formatters;

namespace LightBDD.SummaryGeneration.Configuration
{
    public static class SummaryFileWriterConfigurationExtensions
    {
        public static SummaryWritersConfiguration AddFileWriter<TFormatter>(this SummaryWritersConfiguration cfg, string outputPath) where TFormatter : IResultFormatter, new()
        {
            return cfg.Add(new SummaryFileWriter(new TFormatter(), outputPath));
        }
    }
}