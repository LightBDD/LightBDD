using System;

namespace LightBDD.XUnit2
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ClassCollectionBehaviorAttribute : Attribute
    {
        public bool AllowTestParallelization { get; set; }
    }
}
