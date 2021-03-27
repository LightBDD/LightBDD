using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Feature starting event.
    /// </summary>
    public sealed class FeatureStartingEvent : ProgressEvent
    {
        /// <summary>
        /// Feature Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid FeatureId { get; set; }
    }
}