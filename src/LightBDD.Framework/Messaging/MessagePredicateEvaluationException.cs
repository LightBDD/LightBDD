using System;

namespace LightBDD.Framework.Messaging
{
    public class MessagePredicateEvaluationException : InvalidOperationException
    {
        public MessagePredicateEvaluationException(Exception inner, object messageObject)
            : base($"Unable to evaluate predicate on message {messageObject?.GetType().Name}: {inner?.Message}", inner)
        {
            MessageObject = messageObject;
        }

        public object MessageObject { get; }
    }
}