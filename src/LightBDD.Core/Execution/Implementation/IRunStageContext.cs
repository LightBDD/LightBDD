#nullable enable
using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution.Implementation;

internal interface IRunStageContext
{
    public EngineContext Engine { get; }
    public IMetadataInfo Info { get; }
    public Func<Exception, bool> ShouldAbortSubStepExecution { get; }
    public IDependencyContainer DependencyContainer { get; }
}