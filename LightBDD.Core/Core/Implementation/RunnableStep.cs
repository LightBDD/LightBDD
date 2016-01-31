using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Implementation
{
    internal class RunnableStep
    {
        private readonly IStepInfo _stepInfo;
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        public StepResult Result { get; private set; }

        public RunnableStep(IStepInfo stepInfo, int stepNumber, Func<Exception, ExecutionStatus> exceptionToStatusMapper)
        {
            Result = new StepResult(stepInfo, stepNumber);
            _stepInfo = stepInfo;
            _exceptionToStatusMapper = exceptionToStatusMapper;
        }

        public async Task Invoke(object context)
        {
            try
            {
                await _stepInfo.StepMethod(context, null);
                Result.SetStatus(ExecutionStatus.Passed);
            }
            catch (StepBypassException e)
            {
                Result.SetStatus(ExecutionStatus.Bypassed, e.Message);
            }
            catch (Exception e)
            {
                Result.SetStatus(_exceptionToStatusMapper(e), e.Message);
                throw;
            }
        }
    }
}