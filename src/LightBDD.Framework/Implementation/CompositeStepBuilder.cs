using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.Implementation
{
    [DebuggerStepThrough]
    internal class CompositeStepBuilder : LightBddConfigurationAware, ICompositeStepBuilder, IIntegrableCompositeStepBuilder
    {
        private static readonly IEnumerable<StepDescriptor> EmptySteps = Enumerable.Empty<StepDescriptor>();

        private IEnumerable<StepDescriptor> _steps = EmptySteps;
        private Func<object> _contextProvider;

        public CompositeStep Build()
        {
            return new CompositeStep(_contextProvider ?? ProvideNoContext, _steps);
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));
            _steps = _steps.Concat(steps);
            return this;
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider)
        {
            if (_contextProvider != null || !ReferenceEquals(_steps, EmptySteps))
                throw new InvalidOperationException("Step context can be specified only once, when no steps are specified yet.");

            _contextProvider = contextProvider;
            return this;
        }

        public TEnrichedBuilder Enrich<TEnrichedBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedBuilder> builderFactory)
        {
            return builderFactory(this, Configuration);
        }

        private static object ProvideNoContext() => null;
    }
}