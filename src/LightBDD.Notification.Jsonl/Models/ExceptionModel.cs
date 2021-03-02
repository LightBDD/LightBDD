using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    public class ExceptionModel
    {
        public static ExceptionModel From(Exception ex)
        {
            if (ex == null) return null;
            return new ExceptionModel
            {
                Message = ex.Message,
                Type = ex.GetType().FullName,
                StackTrace = ex.StackTrace,
                InnerException = ExceptionModel.From(ex.InnerException)
            };
        }

        [JsonPropertyName("i")]
        public ExceptionModel InnerException { get; set; }
        [JsonPropertyName("s")]
        public string StackTrace { get; set; }
        [JsonPropertyName("t")]
        public string Type { get; set; }
        [JsonPropertyName("m")]
        public string Message { get; set; }
    }
}