using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    internal class StepNameInfo : NameInfo, IStepNameInfo
    {
        public StepNameInfo(StepNameModel meta) : base(meta)
        {
            OriginalTypeName = meta.OriginalTypeName;
            TypeName = meta.TypeName;
        }

        public string OriginalTypeName { get; }
        public string TypeName { get; }
    }
}