namespace LightBDD.Framework.UnitTests.Messaging.Helpers
{
    class TestMessage
    {
        public TestMessage(string id)
        {
            Id = id;
        }
        public string Id { get; }
        public string Text { get; set; }
        public override string ToString() => Id;
    }
}