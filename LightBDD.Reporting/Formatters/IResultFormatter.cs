using System.IO;
using LightBDD.Core.Results;

namespace LightBDD.Reporting.Formatters
{
    public interface IResultFormatter
    {
        void Format(Stream stream, params IFeatureResult[] features);
    }
}