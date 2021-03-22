using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results.Implementation;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Execution.Implementation
{
    internal class MethodArgument : INameParameterInfo, IParameterResult
    {
        private readonly Func<object, object> _valueEvaluator;
        private readonly IValueFormattingService _formattingService;
        private readonly IProgressPublisher _publisher;
        public bool IsEvaluated { get; private set; }
        public ParameterVerificationStatus VerificationStatus => Details.VerificationStatus;
        public IParameterInfo Info { get; }
        public string FormattedValue { get; private set; } = NameParameterInfo.UnknownValue;
        public string Name => Info.Name;
        public IParameterDetails Details => GetDetails();
        public object Value { get; private set; }

        private IParameterDetails GetDetails()
        {
            if (!IsEvaluated)
                return ParameterDetails.NotEvaluated;
            if (Value is IComplexParameter c)
                return c.Details;
            return ParameterDetails.NotApplicable;
        }

        public MethodArgument(IMetadataInfo owner, ParameterDescriptor descriptor, CoreMetadataProvider metadataProvider, IProgressPublisher publisher)
        {
            Info = new RunnableParameterInfo(owner, descriptor.RawName);
            _formattingService = metadataProvider.GetValueFormattingServiceFor(descriptor.ParameterInfo);
            _publisher = publisher;
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
            if (Value is ITraceableParameter traceable)
                traceable.InitializeParameterTrace(Info, _publisher);
            IsEvaluated = true;
            Reformat();
        }

        public void Reformat()
        {
            try
            {
                if (IsEvaluated)
                    FormattedValue = _formattingService.FormatValue(Value);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Unable to format '{Name}' parameter: {e.Message}");
            }
        }

        public override string ToString()
        {
            return $"{(IsEvaluated ? Value : NameParameterInfo.UnknownValue)}";
        }
    }
}