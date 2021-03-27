using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Scenario discovered event.
    /// </summary>
    public sealed class ScenarioDiscoveredEvent : ProgressEvent
    {
        /// <summary>
        /// Scenario Id
        /// </summary>
        [JsonPropertyName("i")]
        public Guid ScenarioId { get; set; }

        /// <summary>
        /// Scenario name.
        /// </summary>
        [JsonPropertyName("n")]
        public NameModel Name { get; set; }

        /// <summary>
        /// Parent Feature Id
        /// </summary>
        [JsonPropertyName("p")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Scenario labels.
        /// </summary>
        [JsonPropertyName("l")]
        public string[] Labels { get; set; }

        /// <summary>
        /// Scenario categories.
        /// </summary>
        [JsonPropertyName("c")]
        public string[] Categories { get; set; }
    }
}