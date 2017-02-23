using System;
using System.Diagnostics;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class StepParameter
    {
        private readonly Func<object, object> _valueEvaluator;
        private readonly Func<object, string> _valueFormatter;
        public string RawName { get; private set; }
        public bool IsEvaluated { get; private set; }
        public object Value { get; private set; }

        public StepParameter(ParameterDescriptor descriptor, Func<object, string> valueFormatter)
        {
            _valueFormatter = valueFormatter;
            RawName = descriptor.RawName;
            _valueEvaluator = descriptor.ValueEvaluator;
            if (descriptor.IsConstant)
                Evaluate(null);
        }

        public void Evaluate(object context)
        {
            if (IsEvaluated) return;
            Value = _valueEvaluator.Invoke(context);
            IsEvaluated = true;
        }

        public INameParameterInfo FormatNameParameter()
        {
            return IsEvaluated
                ? new NameParameterInfo(true, _valueFormatter.Invoke(Value))
                : NameParameterInfo.Unknown;
        }

        public override string ToString()
        {
            return string.Format("{0}", IsEvaluated ? Value : NameParameterInfo.UnknownValue);
        }
    }
}