namespace LightBDD.UnitTests.Helpers
{
    internal class StepParameterExpectation
    {
        public StepParameterExpectation(string formattedValue, bool isEvaluated)
        {
            IsEvaluated = isEvaluated;
            FormattedValue = formattedValue;
        }

        public string FormattedValue { get; private set; }
        public bool IsEvaluated { get; private set; }
    }
}