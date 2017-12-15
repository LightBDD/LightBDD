using System.Threading.Tasks;

namespace LightBDD.Framework.Notification.Html.Messaging
{
    internal class MessageNode
    {
        public byte[] Data { get; }
        private readonly TaskCompletionSource<MessageNode> _next = new TaskCompletionSource<MessageNode>();

        public MessageNode(byte[] data)
        {
            Data = data;
        }

        public async Task<MessageNode> GetNextAsync()
        {
            return await _next.Task;
        }

        public MessageNode Append(MessageNode next)
        {
            _next.SetResult(next);
            return next;
        }
    }
}