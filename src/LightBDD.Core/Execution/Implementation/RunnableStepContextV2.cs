using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;
using LightBDD.Core.Reporting;

namespace LightBDD.Core.Execution.Implementation;

internal class RunnableStepContextV2
{
    private readonly IRunStageContext _parent;

    public RunnableStepContextV2(IRunStageContext parent, object context)
    {
        Context = context;
        _parent = parent;
    }

    public ExceptionProcessor ExceptionProcessor => _parent.Engine.ExceptionProcessor;
    public IProgressNotifier ProgressNotifier => _parent.Engine.ProgressNotifier;
    public IDependencyContainer Container => _parent.DependencyContainer;
    public object Context { get; }
    public Func<Exception, bool> ShouldAbortSubStepExecution => _parent.ShouldAbortSubStepExecution;
    public IExecutionTimer ExecutionTimer => _parent.Engine.ExecutionTimer;
    public IFileAttachmentsManager FileAttachmentsManager => _parent.Engine.FileAttachmentsManager;
    public EngineContext Engine =>_parent.Engine;
}