using LightBDD.Notification;
using LightBDD.Results;

namespace LightBDD.Execution
{
    internal interface IStep
    {
        IStepResult GetResult();
        void Invoke(IProgressNotifier progressNotifier, int totalCount);
    }
}