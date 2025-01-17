﻿using System;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class CastExpectation<TDerived, TBase> : Expectation<TBase> where TDerived : TBase
    {
        private readonly IExpectation<TDerived> _expectation;

        public CastExpectation(IExpectation<TDerived> expectation)
        {
            _expectation = expectation;
        }

        public override ExpectationResult Verify(TBase value, IValueFormattingService formattingService)
        {
            if (CastHelper.IsAssignableTo<TDerived>(value))
                return _expectation.Verify((TDerived)value, formattingService);

            if (NumericTypeHelper.IsNumeric(typeof(TDerived)) && NumericTypeHelper.IsNumeric(value))
            {
                try
                {
                    if (CastHelper.TryConvertWithoutPrecisionLoss<TDerived>(value, out var derived))
                        return _expectation.Verify(derived, formattingService);
                    return ExpectationResult.Failure($"value '{formattingService.FormatValue(value)}' of type '{value.GetType().Name}' cannot be cast to '{typeof(TDerived).Name}' without precision loss");
                }
                catch (Exception ex)
                {
                    return ExpectationResult.Failure($"value '{formattingService.FormatValue(value)}' of type '{value.GetType().Name}' cannot be cast to '{typeof(TDerived).Name}': {ex.Message}");
                }
            }

            return ExpectationResult.Failure($"value '{formattingService.FormatValue(value)}' of type '{value?.GetType().Name ?? "<null>"}' cannot be cast to '{typeof(TDerived).Name}'");
        }

        public override string Format(IValueFormattingService formattingService)
        {
            return _expectation.Format(formattingService);
        }
    }
}