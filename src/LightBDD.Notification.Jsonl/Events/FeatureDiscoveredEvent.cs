using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Feature discovered event.
    /// </summary>
    public sealed class FeatureDiscoveredEvent : NotificationEvent
    {
        /// <summary>
        /// Feature labels.
        /// </summary>
        [JsonPropertyName("l")]
        public string[] Labels { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Feature description.
        /// </summary>
        [JsonPropertyName("d")]
        public string Description { get; set; }

        /// <summary>
        /// Feature name.
        /// </summary>
        [JsonPropertyName("n")]
        public NameModel Name { get; set; }

        /// <summary>
        /// Feature Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid FeatureId { get; set; }
    }
}