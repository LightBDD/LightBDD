using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Step finished event.
    /// </summary>
    public sealed class StepFinishedEvent : NotificationEvent
    {
        /// <summary>
        /// Step Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid StepId { get; set; }

        /// <summary>
        /// Status details.
        /// </summary>
        [JsonPropertyName("d")]
        public string StatusDetails { get; set; }

        /// <summary>
        /// Status.
        /// </summary>
        [JsonPropertyName("s")]
        public ExecutionStatus Status { get; set; }

        /// <summary>
        /// Step exception (if present).
        /// </summary>
        [JsonPropertyName("e")]
        public ExceptionModel Exception { get; set; }
    }
}