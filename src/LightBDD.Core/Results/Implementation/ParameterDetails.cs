using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Results.Implementation
{
    internal class ParameterDetails : IParameterDetails
    {
        public static readonly ParameterDetails NotEvaluated = new ParameterDetails(ParameterVerificationStatus.NotProvided, "Not evaluated");
        public static readonly ParameterDetails NotApplicable = new ParameterDetails(ParameterVerificationStatus.NotApplicable, null);

        private ParameterDetails(ParameterVerificationStatus status, string message)
        {
            VerificationMessage = message;
            VerificationStatus = status;
        }

        public string VerificationMessage { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
    }
}