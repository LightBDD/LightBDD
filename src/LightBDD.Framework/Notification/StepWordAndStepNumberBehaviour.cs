namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// Controls how the step word (e.g. "STEP") and step number are included in progress notifications.
    /// </summary>
    public enum StepWordAndStepNumberBehaviour
    {
        /// <summary>
        /// Includes the step word and number as a prefix followed by a colon.
        /// <br/>Example: <c>STEP 1.1: Given some condition</c>
        /// </summary>
        IncludeAsPrefix,

        /// <summary>
        /// Includes the step word and number as a suffix in parentheses.
        /// <br/>Example: <c>Given some condition (STEP 1.1)</c>
        /// </summary>
        IncludeAsSuffix,

        /// <summary>
        /// Excludes the step word and number entirely from the notification.
        /// <br/>Example: <c>Given some condition</c>
        /// </summary>
        Exclude
    }
}
