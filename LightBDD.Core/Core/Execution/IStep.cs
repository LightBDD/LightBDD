using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution
{
    public interface IStep
    {
        IStepInfo Info { get; }
        void Comment(string comment);
    }
}