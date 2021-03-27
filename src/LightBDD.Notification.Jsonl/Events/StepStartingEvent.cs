using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Step starting event.
    /// </summary>
    public sealed class StepStartingEvent : ProgressEvent
    {
        /// <summary>
        /// Step Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid StepId { get; set; }
    }
}