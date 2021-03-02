namespace LightBDD.Notification.Jsonl.Events
{
    public sealed class Unknown : Event
    {
        private string _typeCode;

        internal Unknown(string typeCode)
        {
            _typeCode = typeCode;
        }

        public override string TypeCode => _typeCode;
    }
}