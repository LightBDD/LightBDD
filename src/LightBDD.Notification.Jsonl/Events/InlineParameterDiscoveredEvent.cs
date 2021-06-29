using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Parameter discovery event.
    /// </summary>
    public sealed class InlineParameterDiscoveredEvent : NotificationEvent
    {
        /// <summary>
        /// Parameter Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid ParameterId { get; set; }
        /// <summary>
        /// Parameter parent (step).
        /// </summary>
        [JsonPropertyName("p")]
        public Guid ParentId { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        [JsonPropertyName("n")]
        public string Name { get; set; }
        /// <summary>
        /// Expectation
        /// </summary>
        [JsonPropertyName("e")]
        public string Expectation { get; set; }
    }
}