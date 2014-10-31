using System;

namespace LightBDD.Execution.Parameters
{
    internal class MutableStepParameter<TContext> : IStepParameter<TContext>
    {
        private readonly Func<StepType, TContext, object> _function;

        public MutableStepParameter(Func<StepType, TContext, object> function)
        {
            _function = function;
        }

        public object Evaluate(TContext context)
        {
            return _function(StepType.Default, context);
        }

        public object GetNotEvaluatedValue()
        {
            return "<?>";
        }
    }
}