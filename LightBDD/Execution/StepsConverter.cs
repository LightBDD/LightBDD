using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Results;

namespace LightBDD.Execution
{
    internal class StepsConverter : IStepsConverter
    {
        private readonly Func<Type, ResultStatus> _mapExceptionToStatus;
        private readonly TestMetadataProvider _metadataProvider;

        public StepsConverter(TestMetadataProvider metadataProvider, Func<Type, ResultStatus> mapExceptionToStatus)
        {
            _mapExceptionToStatus = mapExceptionToStatus;
            _metadataProvider = metadataProvider;
        }

        public IEnumerable<Step> Convert(IEnumerable<Action> steps)
        {
            int i = 0;
            return steps.Select(step => new Step(step, _metadataProvider.GetStepName(step.Method), ++i, _mapExceptionToStatus));
        }

        public IEnumerable<Step> Convert<TContext>(TContext context, IEnumerable<Action<TContext>> steps)
        {
            int i = 0;
            return steps.Select(step => new Step(() => step(context), _metadataProvider.GetStepName(step.Method), ++i, _mapExceptionToStatus));
        }
    }
}