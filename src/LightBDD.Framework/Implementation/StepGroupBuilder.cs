using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Implementation
{
    internal class StepGroupBuilder : LightBddConfigurationAware, IStepGroupBuilder, IIntegrableStepGroupBuilder
    {
        private static readonly IEnumerable<StepDescriptor> EmptySteps = Enumerable.Empty<StepDescriptor>();

        private IEnumerable<StepDescriptor> _steps = EmptySteps;
        private Func<object> _contextProvider;

        public StepGroup Build()
        {
            return new StepGroup(_contextProvider ?? ProvideNoContext, _steps);
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            _steps = _steps.Concat(steps);
            return this;
        }

        public IIntegrableStepGroupBuilder WithStepContext(Func<object> contextProvider)
        {
            if (_contextProvider != null || !ReferenceEquals(_steps, EmptySteps))
                throw new InvalidOperationException("Step context can be specified only once, when no steps are specified yet.");

            _contextProvider = contextProvider;
            return this;
        }

        public TStepGroupBuilder Enrich<TStepGroupBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory)
        {
            return builderFactory(this, Configuration);
        }

        private static object ProvideNoContext() => null;
    }
}