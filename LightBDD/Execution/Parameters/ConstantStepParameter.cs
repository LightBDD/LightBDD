namespace LightBDD.Execution.Parameters
{
    internal class ConstantStepParameter<TContext> : IStepParameter<TContext>
    {
        private readonly object _value;

        public ConstantStepParameter(object value)
        {
            _value = value;
        }

        public object Evaluate(TContext context)
        {
            return _value;
        }

        public bool IsSafelyEvaluable()
        {
            return true;
        }
    }
}