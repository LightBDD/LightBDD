using LightBDD.Notification.Jsonl.Models;
using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Parameter validation finished event.
    /// </summary>
    public sealed class InlineParameterValidationFinishedEvent : NotificationEvent
    {
        /// <summary>
        /// Parameter Id.
        /// </summary>
        [JsonPropertyName("i")]
        public Guid ParameterId { get; set; }
        /// <summary>
        /// Expectation
        /// </summary>
        [JsonPropertyName("e")]
        public string Expectation { get; set; }
        /// <summary>
        /// Actual value
        /// </summary>
        [JsonPropertyName("v")]
        public string Value { get; set; }
        /// <summary>
        /// Verification status.
        /// </summary>
        [JsonPropertyName("s")]
        public ParameterVerificationStatus VerificationStatus { get; set; }
        /// <summary>
        /// Verification message.
        /// </summary>
        [JsonPropertyName("m")]
        public string VerificationMessage { get; set; }
    }
}