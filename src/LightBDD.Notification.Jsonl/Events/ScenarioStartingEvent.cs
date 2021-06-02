using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Scenario starting event.
    /// </summary>
    public sealed class ScenarioStartingEvent : NotificationEvent
    {
        /// <summary>
        /// Scenario Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid ScenarioId { get; set; }
    }
}