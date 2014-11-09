using System.Diagnostics;

namespace LightBDD.Results.Implementation
{
    [DebuggerStepThrough]
    internal class StepParameter : IStepParameter
    {
        public StepParameter(bool isEvaluated, string formattedValue)
        {
            FormattedValue = formattedValue;
            IsEvaluated = isEvaluated;
        }

        public bool IsEvaluated { get; private set; }
        public string FormattedValue { get; private set; }
    }
}