namespace LightBDD.Notification.Jsonl.Events
{
    /// <summary>
    /// Unknown event.
    /// </summary>
    public sealed class UnknownEvent : NotificationEvent
    {
        internal UnknownEvent(string typeCode)
            : base(typeCode)
        {
        }
    }
}