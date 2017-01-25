namespace LightBDD.Core.Metadata.Implementation
{
    internal class NameParameterInfo : INameParameterInfo
    {
        public const string UnknownValue = "<?>";
        public static readonly INameParameterInfo Unknown = new NameParameterInfo(false, UnknownValue);

        public NameParameterInfo(bool isEvaluated, string formattedValue)
        {
            IsEvaluated = isEvaluated;
            FormattedValue = formattedValue;
        }

        public bool IsEvaluated { get; }
        public string FormattedValue { get; }
    }
}