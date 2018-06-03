using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Results.Implementation
{
    internal class ParameterResult : IParameterResult
    {
        public string Name { get; }
        public IParameterDetails Details { get; }

        public ParameterResult(string name, IParameterDetails result)
        {
            Name = name;
            Details = result;
        }
    }
}