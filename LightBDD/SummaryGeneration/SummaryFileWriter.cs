using System.IO;
using System.Text;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.SummaryGeneration.Implementation;

namespace LightBDD.SummaryGeneration
{
    /// <summary>
    /// Summary file writer class allows to save feature results by using associated result formatter and output path.
    /// </summary>
    public class SummaryFileWriter : ISummaryWriter
    {
        private readonly IResultFormatter _formatter;
        private readonly string _outputPath;

        /// <summary>
        /// Constructor allowing to create SummaryFileWriter with associated result formatter and output path.
        ///  Please note that full output path is being resolved at time when constructor is called, not when results are saved, so any relative paths will be resolved at the construction of this class.
        /// </summary>
        /// <param name="formatter">Result formatter.</param>
        /// <param name="outputPath">Output path. If starts with ~, it would be resolved to CurrentDomain.BaseDirectory.</param>
        public SummaryFileWriter(IResultFormatter formatter, string outputPath)
        {
            _outputPath = Path.GetFullPath(FilePathHelper.ResolveAbsolutePath(outputPath));
            _formatter = formatter;
        }

        /// <summary>
        /// Saves formatted feature <c>results</c> to file specified in constructor.
        /// If output path refers to directory which does not exist, it will be created.
        /// </summary>
        /// <param name="results">Results to save.</param>
        public void Save(params IFeatureResult[] results)
        {
            FilePathHelper.EnsureOutputDirectoryExists(_outputPath);
            File.WriteAllText(_outputPath, _formatter.Format(results), Encoding.UTF8);
        }
    }
}