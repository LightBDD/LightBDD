using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Feature finished event.
    /// </summary>
    public sealed class FeatureFinishedEvent : NotificationEvent
    {
        /// <summary>
        /// Feature Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid FeatureId { get; set; }
    }
}