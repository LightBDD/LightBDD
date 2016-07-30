using System;
using System.IO;
using LightBDD.Core.Execution.Results;
using LightBDD.SummaryGeneration.Formatters;

namespace LightBDD.SummaryGeneration
{
    /// <summary>
    /// Summary file writer class allows to save feature results by using associated result formatter and output path.
    /// </summary>
    public class SummaryFileWriter : ISummaryWriter
    {
        public IResultFormatter Formatter { get; }
        public string OutputPath { get; }
        public string FullOutputPath { get; }

        /// <summary>
        /// Constructor allowing to create SummaryFileWriter with associated result formatter and output path.
        /// </summary>
        /// <param name="formatter">Result formatter.</param>
        /// <param name="outputPath">Output path.</param>
        public SummaryFileWriter(IResultFormatter formatter, string outputPath)
        {
            OutputPath = outputPath;
            Formatter = formatter;
            FullOutputPath = GetOutputPath(outputPath);
        }

        private static string GetOutputPath(string outputPath)
        {
            if (outputPath.StartsWith("~"))
                outputPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + outputPath.Substring(1);
            return Path.GetFullPath(outputPath);
        }

        /// <summary>
        /// Saves formatted feature <c>results</c> to file specified in constructor.
        /// If output path refers to directory which does not exist, it will be created.
        /// </summary>
        /// <param name="results">Results to save.</param>
        public void Save(params IFeatureResult[] results)
        {
            EnsureOutputDirectoryExists();
            using (var stream = File.Create(FullOutputPath))
                Formatter.Format(stream, results);
        }

        private void EnsureOutputDirectoryExists()
        {
            var directory = Path.GetDirectoryName(FullOutputPath);
            if (directory != null)
                Directory.CreateDirectory(directory);
        }
    }
}