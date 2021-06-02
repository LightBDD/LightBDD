using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Scenario finished event.
    /// </summary>
    public sealed class ScenarioFinishedEvent : NotificationEvent
    {
        /// <summary>
        /// Scenario Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid ScenarioId { get; set; }

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
    }
}