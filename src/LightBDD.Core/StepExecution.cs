using LightBDD.Core.Execution;

namespace LightBDD
{
    public class StepExecution
    {
        public static StepExecution Current { get; } = new StepExecution();

        private StepExecution() { }

        public void Bypass(string reason)
        {
            throw new StepBypassException(reason);
        }
    }
}