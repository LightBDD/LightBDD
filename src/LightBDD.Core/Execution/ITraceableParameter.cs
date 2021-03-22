using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Interface describing a method parameter with traceable behaviors.
    /// </summary>
    public interface ITraceableParameter
    {
        /// <summary>
        /// Initializes the parameter with <paramref name="parameterInfo"/> and <paramref name="progressPublisher"/>
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <param name="progressPublisher"></param>
        void InitializeParameterTrace(IParameterInfo parameterInfo, IProgressPublisher progressPublisher);
    }
}