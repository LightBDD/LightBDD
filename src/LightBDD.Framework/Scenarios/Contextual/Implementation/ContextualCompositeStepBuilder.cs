using System;
using System.Collections.Generic;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualCompositeStepBuilder<TContext> : LightBddConfigurationAware, ICompositeStepBuilder<TContext>, IIntegrableCompositeStepBuilder
    {
        private readonly IIntegrableCompositeStepBuilder _target;

        public ContextualCompositeStepBuilder(ICompositeStepBuilder runner, Func<object> contextProvider, bool takeOwnership)
        {
            _target = runner.Integrate().WithStepContext(contextProvider, takeOwnership);
        }

        public ContextualCompositeStepBuilder(ICompositeStepBuilder runner, Func<IDependencyResolver, object> contextResolver)
        {
            _target = runner.Integrate().WithStepContext(contextResolver);
        }

        public CompositeStep Build()
        {
            return _target.Build();
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            _target.AddSteps(steps);
            return this;
        }

        LightBddConfiguration IIntegrableStepGroupBuilder.Configuration => Configuration;

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider, bool takeOwnership)
        {
            _target.WithStepContext(contextProvider, takeOwnership);
            return this;
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator)
        {
            _target.WithStepContext(contextProvider, scopeConfigurator);
            return this;
        }
    }
}