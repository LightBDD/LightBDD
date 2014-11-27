using LightBDD.Notification;
using LightBDD.Results;

namespace LightBDD.Execution.Implementation
{
    internal interface IStep
    {
        IStepResult GetResult();
        void Invoke(IProgressNotifier progressNotifier, int totalCount);
    }
}