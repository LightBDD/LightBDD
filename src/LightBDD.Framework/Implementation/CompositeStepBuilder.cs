#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.Implementation
{
    internal class CompositeStepBuilder : LightBddConfigurationAware, ICompositeStepBuilder, IIntegrableCompositeStepBuilder
    {
        private static readonly IEnumerable<StepDescriptor> EmptySteps = Enumerable.Empty<StepDescriptor>();

        private IEnumerable<StepDescriptor> _steps = EmptySteps;
        private Resolvable<object?>? _contextDescriptor;

        public CompositeStep Build()
        {
            return new CompositeStep(_contextDescriptor??Resolvable<object?>.Null, _steps);
        }

        public IIntegrableCompositeStepBuilder Integrate()
        {
            return this;
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));
            _steps = _steps.Concat(steps);
            return this;
        }

        LightBddConfiguration IIntegrableStepGroupBuilder.Configuration => Configuration;

        public IIntegrableCompositeStepBuilder WithStepContext(Func<IDependencyResolver, object?> contextProvider)
        {
            return WithStepContext(Resolvable.Use(contextProvider));
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object?> contextProvider, bool takeOwnership)
        {
            return WithStepContext(Resolvable.Use(contextProvider, takeOwnership));
        }

        private IIntegrableCompositeStepBuilder WithStepContext(Resolvable<object?> contextProvider)
        {
            if (_contextDescriptor != null || !ReferenceEquals(_steps, EmptySteps))
                throw new InvalidOperationException("Step context can be specified only once, when no steps are specified yet.");

            _contextDescriptor = contextProvider;
            return this;
        }
    }
}