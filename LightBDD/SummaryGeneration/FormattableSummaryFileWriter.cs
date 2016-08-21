using System.IO;
using System.Text;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.SummaryGeneration.Implementation;

namespace LightBDD.SummaryGeneration
{
    /// <summary>
    /// Summary file writer class allowing to save feature results by using associated result formatter and output path.
    /// The output file path will can be represented with a formattable string, that allows to format output path with such parameters as current date time or test execution start time.
    /// </summary>
    public class FormattableSummaryFileWriter : ISummaryWriter
    {
        private readonly IResultFormatter _formatter;
        private readonly SummaryFileFormattablePath _path;

        /// <summary>
        /// Constructor allowing to create SummaryFileWriter with associated result formatter and output path format.
        /// Please note that full output path is being resolved at time when constructor is called, not when results are saved, so any relative paths will be resolved at the construction of this class.
        /// </summary>
        /// <param name="formatter">Result formatter.</param>
        /// <param name="outputPathFormat">Output path. If starts with ~, it would be resolved to CurrentDomain.BaseDirectory. It can contain string.Format() like parameters of {name:format} syntax.
        /// This constructor uses default SummaryFilePathFormatter to format parameters. See <see cref="SummaryFilePathFormatter.CreateDefault"/>() for more details on available parameter types.</param>
        public FormattableSummaryFileWriter(IResultFormatter formatter, string outputPathFormat)
            : this(formatter, outputPathFormat, SummaryFilePathFormatter.CreateDefault()) { }

        /// <summary>
        /// Constructor allowing to create SummaryFileWriter with associated result formatter and output path format.
        /// Please note that full output path is being resolved at time when constructor is called, not when results are saved, so any relative paths will be resolved at the construction of this class.
        /// </summary>
        /// <param name="formatter">Result formatter.</param>
        /// <param name="outputPathFormat">Output path. If starts with ~, it would be resolved to CurrentDomain.BaseDirectory. It can contain string.Format() like parameters of {name:format} syntax.</param>
        /// <param name="pathFormatter">Path formatter used to format <c>outputPathFormat</c>.</param>
        protected FormattableSummaryFileWriter(IResultFormatter formatter, string outputPathFormat, SummaryFilePathFormatter pathFormatter)
        {
            _formatter = formatter;
            _path = pathFormatter.ToFormattablePath(outputPathFormat);
        }

        /// <summary>
        /// Saves formatted feature <c>results</c> to file specified in constructor.
        /// If output path refers to directory which does not exist, it will be created.
        /// </summary>
        /// <param name="results">Results to save.</param>
        public void Save(params IFeatureResult[] results)
        {
            var outputPath = _path.Resolve(results);
            FilePathHelper.EnsureOutputDirectoryExists(outputPath);
            File.WriteAllText(outputPath, _formatter.Format(results), Encoding.UTF8);
        }
    }
}