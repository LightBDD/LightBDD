using System;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class ValueResult : IValueResult
    {
        public ValueResult(string expectation, string value, ParameterVerificationStatus verificationStatus, string message)
        {
            Expectation = expectation;
            Value = value;
            VerificationStatus = verificationStatus;
            Message = message;
        }

        public string Value { get; }
        public string Expectation { get; }
        public string Message { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
    }
}