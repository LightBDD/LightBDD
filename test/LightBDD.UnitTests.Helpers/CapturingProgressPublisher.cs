using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Notification.Events;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    public class CapturingProgressPublisher : IProgressPublisher
    {
        private readonly Stopwatch _watch = Stopwatch.StartNew();
        private readonly IDictionary<Type, Func<ProgressEvent, string>> _loggers = new Dictionary<Type, Func<ProgressEvent, string>>();
        private readonly ConcurrentQueue<string> _eventLogs = new ConcurrentQueue<string>();
        public IEnumerable<string> GetLogs() => _eventLogs;
        public void AssertLogs(params string[] expected)
        {
            var actual = GetLogs().ToArray();
            Assert.That(actual, Is.EqualTo(expected), () => $"expected:\n{string.Join("\n", expected)}\n\ngot:\n{string.Join("\n", actual)}");
        }

        public void Publish<TEvent>(Func<EventTime, TEvent> eventFn) where TEvent : ProgressEvent
        {
            var e = eventFn(new EventTime(DateTimeOffset.MinValue, _watch.Elapsed));
            _eventLogs.Enqueue(Log(e));
        }

        public void RegisterLogger<TEvent>(Func<TEvent, string> log) where TEvent : ProgressEvent => _loggers[typeof(TEvent)] = e => log((TEvent)e);

        public CapturingProgressPublisher()
        {
            string DumpColumns(ITabularParameterDetails d) => string.Join(",", d.Columns.Select(c => c.Name));
            string DumpRows(ITabularParameterDetails d) => string.Join(",", d.Rows.Select(r=>$"{{{r.Type}|{r.VerificationStatus}|[{string.Join(",",r.Values.Select(v=>$"{v.Expectation}/{v.Value}"))}]}}"));

            RegisterLogger<TabularParameterDiscovered>(e => $"{e.GetType().Name}|Param={e.Parameter.Name}|Status={e.Details.VerificationStatus}|Columns=[{DumpColumns(e.Details)}]|Rows=[{DumpRows(e.Details)}]");
            RegisterLogger<TabularParameterValidationStarting>(e => $"{e.GetType().Name}|Param={e.Parameter.Name}|Status={e.Details.VerificationStatus}|Columns=[{DumpColumns(e.Details)}]|Rows=[{DumpRows(e.Details)}]");
            RegisterLogger<TabularParameterValidationFinished>(e => $"{e.GetType().Name}|Param={e.Parameter.Name}|Status={e.Details.VerificationStatus}|Columns=[{DumpColumns(e.Details)}]|Rows=[{DumpRows(e.Details)}]");
            RegisterLogger<InlineParameterDiscovered>(e => $"{e.GetType().Name}|Param={e.Parameter.Name}|Status={e.Details.VerificationStatus}|E={e.Details.Expectation}|V={e.Details.Value}");
            RegisterLogger<InlineParameterValidationStarting>(e => $"{e.GetType().Name}|Param={e.Parameter.Name}|Status={e.Details.VerificationStatus}|E={e.Details.Expectation}|V={e.Details.Value}");
            RegisterLogger<InlineParameterValidationFinished>(e => $"{e.GetType().Name}|Param={e.Parameter.Name}|Status={e.Details.VerificationStatus}|E={e.Details.Expectation}|V={e.Details.Value}");
        }

        private string Log(ProgressEvent e) => _loggers.TryGetValue(e.GetType(), out var logger) ? logger(e) : e.GetType().Name;
    }
}