using LightBDD.Reporting.Formatters;

namespace LightBDD.Reporting.Configuration
{
    public static class ReportFileWriterConfigurationExtensions
    {
        public static ReportWritersConfiguration AddFileWriter<TFormatter>(this ReportWritersConfiguration cfg, string outputPath) where TFormatter : IReportFormatter, new()
        {
            return cfg.Add(new ReportFileWriter(new TFormatter(), outputPath));
        }
    }
}