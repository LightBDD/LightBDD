using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Test Execution starting event.
    /// </summary>
    public sealed class ExecutionStartingEvent : ProgressEvent
    {
        /// <summary>
        /// Start time.
        /// </summary>
        [JsonPropertyName("s")]
        public DateTimeOffset Start { get; set; }
    }
}