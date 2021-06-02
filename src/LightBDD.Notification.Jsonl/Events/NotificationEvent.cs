using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Implementation;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Notification event.
    /// </summary>
    public abstract class NotificationEvent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected NotificationEvent(string typeCode = null)
        {
            TypeCode = typeCode ?? EventMapper.GetCode(GetType());
        }

        /// <summary>
        /// Type Code.
        /// </summary>
        [JsonPropertyName("_c")]
        public string TypeCode { get; }

        /// <summary>
        /// Event time.
        /// </summary>
        [JsonPropertyName("_t")]
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Time { get; set; }
    }
}