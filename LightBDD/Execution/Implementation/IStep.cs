using LightBDD.Notification;
using LightBDD.Results;

namespace LightBDD.Execution.Implementation
{
    internal interface IStep
    {
        IStepResult GetResult();
        void Invoke(ExecutionContext context);
        void Comment(ExecutionContext context, string comment);
    }
}