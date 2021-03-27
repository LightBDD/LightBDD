using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    /// <summary>
    /// Name parameter model.
    /// </summary>
    public class NameParameterModel
    {
        /// <summary>
        /// Verification Status.
        /// </summary>
        [JsonPropertyName("s")]
        public ParameterVerificationStatus VerificationStatus { get; set; }

        /// <summary>
        /// Formatted value.
        /// </summary>
        [JsonPropertyName("v")]
        public string FormattedValue { get; set; }

        /// <summary>
        /// Determines if name parameter is evaluated.
        /// </summary>
        [JsonPropertyName("e")]
        public bool IsEvaluated { get; set; }
    }
}