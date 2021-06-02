using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Step commented event.
    /// </summary>
    public sealed class StepCommentedEvent : NotificationEvent
    {
        /// <summary>
        /// Step Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid StepId { get; set; }

        /// <summary>
        /// Comment.
        /// </summary>
        [JsonPropertyName("c")]
        public string Comment { get; set; }
    }
}