using System.Diagnostics;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    [DebuggerStepThrough]
    internal class ValueResult : IValueResult
    {
        public ValueResult(string expectation, string value, ParameterVerificationStatus verificationStatus, string message)
        {
            Expectation = expectation;
            Value = value;
            VerificationStatus = verificationStatus;
            VerificationMessage = message;
        }

        public string Value { get; }
        public string Expectation { get; }
        public string VerificationMessage { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
    }
}