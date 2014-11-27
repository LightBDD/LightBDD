using System;
using System.Diagnostics;

namespace LightBDD.Execution.Implementation.Parameters
{
    [DebuggerStepThrough]
    internal class MutableStepParameter<TContext> : IStepParameter<TContext>
    {
        private readonly Func<StepType, TContext, object> _function;
        private readonly Func<object, string> _formatFunction;

        public MutableStepParameter(Func<StepType, TContext, object> function, Func<object, string> formatFunction)
        {
            _function = function;
            _formatFunction = formatFunction;
            IsEvaluated = false;
        }

        public void Evaluate(TContext context)
        {
            if (IsEvaluated)
                return;
            IsEvaluated = true;
            Value = _function(StepType.Default, context);
        }

        public bool IsEvaluated { get; private set; }
        public object Value { get; private set; }

        public string Format()
        {
            return IsEvaluated ? _formatFunction(Value) : "<?>";
        }

        public override string ToString()
        {
            return string.Format("{0}", IsEvaluated ? Value : "<?>");
        }
    }
}