using System.Diagnostics;

namespace LightBDD.Core.Metadata.Implementation
{
    [DebuggerStepThrough]
    internal class NameParameterInfo : INameParameterInfo
    {
        public const string UnknownValue = "<?>";
        public static readonly INameParameterInfo Unknown = new NameParameterInfo(false, UnknownValue, ParameterVerificationStatus.NotApplicable);

        public NameParameterInfo(bool isEvaluated, string formattedValue, ParameterVerificationStatus verificationStatus)
        {
            IsEvaluated = isEvaluated;
            FormattedValue = formattedValue;
            VerificationStatus = verificationStatus;
        }

        public bool IsEvaluated { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
        public string FormattedValue { get; }
    }
}