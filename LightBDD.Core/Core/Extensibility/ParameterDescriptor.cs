using System;

namespace LightBDD.Core.Extensibility
{
    public class ParameterDescriptor
    {
        public ParameterDescriptor(bool isConstant, string rawName, Func<object, object> valueEvaluator, Func<object, string> valueFormatter)
        {
            RawName = rawName;
            IsConstant = isConstant;
            ValueEvaluator = valueEvaluator;
            ValueFormatter = valueFormatter;
        }

        public string RawName { get; private set; }
        public bool IsConstant { get; private set; }
        public Func<object, object> ValueEvaluator { get; private set; }
        public Func<object, string> ValueFormatter { get; private set; }
    }
}