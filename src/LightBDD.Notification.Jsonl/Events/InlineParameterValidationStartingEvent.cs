using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Parameter validation start event.
    /// </summary>
    public sealed class InlineParameterValidationStartingEvent : NotificationEvent
    {
        /// <summary>
        /// Parameter Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid ParameterId { get; set; }
    }
}