using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    internal class NameInfo : INameInfo
    {
        private readonly NameModel _meta;

        public NameInfo(NameModel meta)
        {
            _meta = meta;
        }
        public string Name => _meta.Format;
    }
}