namespace LightBDD.Core.Metadata.Implementation
{
    //TODO: remove
    internal class NameParameterInfo : INameParameterInfo
    {
        public const string UnknownValue = "<?>";
        public static INameParameterInfo Unknown(string name) => new NameParameterInfo(name, false, UnknownValue, ParameterVerificationStatus.NotApplicable);

        public NameParameterInfo(string name, bool isEvaluated, string formattedValue, ParameterVerificationStatus verificationStatus)
        {
            Name = name;
            IsEvaluated = isEvaluated;
            FormattedValue = formattedValue;
            VerificationStatus = verificationStatus;
        }

        public bool IsEvaluated { get; }
        public ParameterVerificationStatus VerificationStatus { get; }
        public string FormattedValue { get; }
        public string Name { get; }
    }
}