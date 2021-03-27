using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Notification.Jsonl.Events;

namespace LightBDD.Notification.Jsonl.Implementation
{
    internal class EventMapper
    {
        private static readonly IReadOnlyDictionary<string, Type> CodeTypeMapping;
        private static readonly IReadOnlyDictionary<Type, string> TypeCodeMapping;

        static EventMapper()
        {
            CodeTypeMapping = typeof(EventMapper).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(ProgressEvent).IsAssignableFrom(t))
                .ToDictionary(ToCodeName);
            TypeCodeMapping = CodeTypeMapping.ToDictionary(p => p.Value, p => p.Key);
        }

        private static string ToCodeName(Type t)
        {
            var name = t.Name;
            var suffix = "event";
            return name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
                ? name.Substring(0, name.Length - suffix.Length)
                : name;
        }

        public static string GetCode(Type eventType)
        {
            return TypeCodeMapping.TryGetValue(eventType, out var code)
                ? code
                : throw new KeyNotFoundException($"No event of type {eventType} is registered");
        }

        public static bool TryGetType(string code, out Type eventType) => CodeTypeMapping.TryGetValue(code, out eventType);
    }
}