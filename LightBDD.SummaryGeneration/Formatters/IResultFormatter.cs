using System.IO;
using LightBDD.Core.Execution.Results;

namespace LightBDD.SummaryGeneration.Formatters
{
    public interface IResultFormatter
    {
        void Format(Stream stream, params IFeatureResult[] features);
    }
}