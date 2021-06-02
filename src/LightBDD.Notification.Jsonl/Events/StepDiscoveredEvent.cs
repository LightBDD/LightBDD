using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Step discovered event.
    /// </summary>
    public sealed class StepDiscoveredEvent : NotificationEvent
    {
        /// <summary>
        /// Step Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid StepId { get; set; }

        /// <summary>
        /// Step Parent Id (either step or scenario).
        /// </summary>
        [JsonPropertyName("p")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Step name.
        /// </summary>
        [JsonPropertyName("n")]
        public StepNameModel Name { get; set; }

        /// <summary>
        /// Step number in the group.
        /// </summary>
        [JsonPropertyName("u")]
        public int Number { get; set; }

        /// <summary>
        /// Group prefix.
        /// </summary>
        [JsonPropertyName("g")]
        public string GroupPrefix { get; set; }
    }
}