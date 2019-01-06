using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
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
        private ExecutionContextDescriptor _contextDescriptor;

        public CompositeStep Build()
        {
            return new CompositeStep(_contextDescriptor ?? ExecutionContextDescriptor.NoContext, _steps);
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

        public IIntegrableCompositeStepBuilder WithStepContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator)
        {
            return WithStepContext(new ExecutionContextDescriptor(contextProvider, scopeConfigurator));
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider, bool takeOwnership)
        {
            return WithStepContext(new ExecutionContextDescriptor(contextProvider, takeOwnership));
        }

        private IIntegrableCompositeStepBuilder WithStepContext(ExecutionContextDescriptor contextDescriptor)
        {
            if (_contextDescriptor != null || !ReferenceEquals(_steps, EmptySteps))
                throw new InvalidOperationException("Step context can be specified only once, when no steps are specified yet.");

            _contextDescriptor = contextDescriptor;
            return this;
        }
    }
}