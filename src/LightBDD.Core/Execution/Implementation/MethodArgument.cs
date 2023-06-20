using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Execution.Implementation
{
    internal class MethodArgument
    {
        private readonly Func<object, object> _valueEvaluator;
        private readonly IValueFormattingService _formattingService;
        public string RawName { get; }
        public bool IsEvaluated { get; private set; }
        public object Value { get; private set; }

        public MethodArgument(ParameterDescriptor descriptor, IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
            RawName = descriptor.RawName;
            _valueEvaluator = descriptor.ValueEvaluator;
            if (descriptor.IsConstant)
                Evaluate(null);
        }

        public void Evaluate(object context)
        {
            if (IsEvaluated)
                return;

            Value = _valueEvaluator.Invoke(context);
            if (Value is IComplexParameter complex)
                complex.SetValueFormattingService(_formattingService);
            IsEvaluated = true;
        }

        public INameParameterInfo FormatNameParameter()
        {
            if (!IsEvaluated)
                return NameParameterInfo.Unknown;

            var value = Value is IComplexParameter { Details: not IInlineParameterDetails }
                ? $"<${RawName}>"
                : _formattingService.FormatValue(Value);

            var status = Value is IComplexParameter cx
                ? cx.Details.VerificationStatus
                : ParameterVerificationStatus.NotApplicable;

            return new NameParameterInfo(true, value, status);
        }

        public override string ToString()
        {
            return $"{(IsEvaluated ? Value : NameParameterInfo.UnknownValue)}";
        }
    }
}