using System;
using LightBDD.Core.Formatting.Diagnostics;

namespace LightBDD.Framework.Messaging
{
    /// <summary>
    /// Exception thrown by <seealso cref="MessageListener"/> when specified message object caused predicate evaluation to throw.
    /// </summary>
    public class MessagePredicateEvaluationException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="innerException">Predicate evaluation exception.</param>
        /// <param name="messageObject">Message object caused predicate evaluation to throw.</param>
        public MessagePredicateEvaluationException(Exception innerException, object messageObject)
            : base($"Unable to evaluate predicate on message {messageObject?.GetType().Name}: {innerException?.Message}\nFaulty message:\n{ObjectFormatter.Dump(messageObject)}", innerException)
        {
            MessageObject = messageObject;
        }

        /// <summary>
        /// Message object caused predicate evaluation to throw.
        /// </summary>
        public object MessageObject { get; }
    }
}