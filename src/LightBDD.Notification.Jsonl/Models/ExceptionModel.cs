using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    /// <summary>
    /// Exception model.
    /// </summary>
    public class ExceptionModel
    {
        /// <summary>
        /// Inner exception (if present).
        /// </summary>
        [JsonPropertyName("i")]
        public ExceptionModel InnerException { get; set; }

        /// <summary>
        /// Stack trace.
        /// </summary>
        [JsonPropertyName("s")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Exception type.
        /// </summary>
        [JsonPropertyName("t")]
        public string Type { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        [JsonPropertyName("m")]
        public string Message { get; set; }
    }
}