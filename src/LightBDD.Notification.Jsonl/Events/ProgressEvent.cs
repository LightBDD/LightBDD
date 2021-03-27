using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Implementation;

namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Progress event.
    /// </summary>
    public abstract class ProgressEvent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected ProgressEvent(string typeCode = null)
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