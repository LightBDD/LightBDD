using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Results.Implementation
{
    internal class ParameterResult : IParameterResult
    {
        public string Name { get; }
        public IParameterVerificationResult Result { get; }

        public ParameterResult(string name, IParameterVerificationResult result)
        {
            Name = name;
            Result = result;
        }
    }
}