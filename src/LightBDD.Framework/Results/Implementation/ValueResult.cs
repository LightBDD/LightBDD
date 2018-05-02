using System;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Results.Implementation
{
    internal class ValueResult : IValueResult
    {
        public ValueResult(string expectation, string value, ParameterVerificationStatus verificationStatus, Exception exception)
        {
            Expectation = expectation;
            Value = value;
            VerificationStatus = verificationStatus;
            Exception = exception;
        }

        public string Value { get; }
        public string Expectation { get; }
        public Exception Exception { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
    }
}