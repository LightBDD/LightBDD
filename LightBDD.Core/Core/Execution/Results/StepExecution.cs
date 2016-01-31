namespace LightBDD.Core.Execution.Results
{
    public class StepExecution
    {
        private static readonly StepExecution Instance = new StepExecution();

        public static StepExecution Current { get { return Instance; } }

        private StepExecution() { }

        public void Bypass(string reason)
        {
            throw new StepBypassException(reason);
        }
    }
}