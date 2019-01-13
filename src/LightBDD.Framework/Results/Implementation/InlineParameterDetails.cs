using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class InlineParameterDetails : IInlineParameterDetails
    {
        public InlineParameterDetails(string expectation, string value, ParameterVerificationStatus verificationStatus, string message)
        {
            Expectation = expectation;
            Value = value;
            VerificationStatus = verificationStatus;
            VerificationMessage = message;
        }

        public string VerificationMessage { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
        public string Value { get; }
        public string Expectation { get; }
    }
}