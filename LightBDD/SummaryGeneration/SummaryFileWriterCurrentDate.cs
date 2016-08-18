using System;
using System.IO;
using System.Text;
using LightBDD.Results;
using LightBDD.Results.Formatters;

namespace LightBDD.SummaryGeneration
{
    /// <summary>
    /// Summary file writer class allows to save feature results by using associated result formatter and output path.
    /// The output file will be formatted with the current date and time.
    /// </summary>
    public class SummaryFileWriterCurrentDate : ISummaryWriter
    {
        private readonly SummaryFileWriter _writer;

        /// <summary>
        /// Constructor allowing to create SummaryFileWriter with associated result formatter and output path.
        /// </summary>
        /// <param name="formatter">Result formatter.</param>
        /// <param name="outputPath">Output path. If starts with ~, it would be resolved to CurrentDomain.BaseDirectory.</param>
        public SummaryFileWriterCurrentDate(IResultFormatter formatter, string outputPath)
        {
            _writer = new SummaryFileWriter(formatter, string.Format(outputPath, DateTime.Now));
        }
        
        /// <summary>
        /// Saves formatted feature <c>results</c> to file specified in constructor.
        /// If output path refers to directory which does not exist, it will be created.
        /// </summary>
        /// <param name="results">Results to save.</param>
        public void Save(params IFeatureResult[] results)
        {
            _writer.Save(results);
        }
    }
}