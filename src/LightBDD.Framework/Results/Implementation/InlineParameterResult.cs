using System;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class InlineParameterResult : IInlineParameterResult
    {
        public InlineParameterResult(string expectation, string value, ParameterVerificationStatus verificationStatus, string message)
        {
            Expectation = expectation;
            Value = value;
            VerificationStatus = verificationStatus;
            Message = message;
        }

        public string Message { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
        public string Value { get; }
        public string Expectation { get; }
    }
}