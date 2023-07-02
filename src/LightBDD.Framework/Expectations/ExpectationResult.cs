namespace LightBDD.Framework.Expectations
{
    /// <summary>
    /// Expectation verification result.
    /// </summary>
    public struct ExpectationResult
    {
        /// <summary>
        /// Default result representing success.
        /// </summary>
        public static ExpectationResult Success { get; } = new(true, string.Empty);

        /// <summary>
        /// Returns <c>true</c> if verification passed.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Returns verification message, provided upon failure.
        /// </summary>
        public string Message { get; }

        private ExpectationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        /// <summary>
        /// Returns <c>true</c> if verification passed.
        /// </summary>
        public static implicit operator bool(ExpectationResult ver) => ver.IsValid;
        /// <summary>
        /// Returns <c>true</c> if verification passed.
        /// </summary>
        public static bool operator true(ExpectationResult ver) => ver.IsValid;
        /// <summary>
        /// Returns <c>true</c> if verification failed.
        /// </summary>
        public static bool operator false(ExpectationResult ver) => !ver.IsValid;

        /// <summary>
        /// Creates result representing verification failure.
        /// </summary>
        /// <param name="message">Failure message</param>
        public static ExpectationResult Failure(string message)
        {
            return new ExpectationResult(false, message);
        }
    }
}