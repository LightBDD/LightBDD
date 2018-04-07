namespace LightBDD.Framework.Expectations
{
    public struct ExpectationResult
    {
        public static ExpectationResult Success { get; } = new ExpectationResult(true, string.Empty);
        public bool IsValid { get; }
        public string Message { get; }

        private ExpectationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public static implicit operator bool(ExpectationResult ver) => ver.IsValid;
        public static bool operator true(ExpectationResult ver) => ver.IsValid;
        public static bool operator false(ExpectationResult ver) => !ver.IsValid;

        public static ExpectationResult Failure(string message)
        {
            return new ExpectationResult(false, message);
        }
    }
}