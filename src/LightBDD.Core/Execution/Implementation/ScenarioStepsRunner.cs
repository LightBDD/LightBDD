#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation;

internal class ScenarioStepsRunner : ICoreScenarioStepsRunner
{
    private readonly IRunStageContext _parent;
    private IEnumerable<StepDescriptor> _stepDescriptors = Enumerable.Empty<StepDescriptor>();
    private ExecutionContextDescriptor _contextDescriptor = ExecutionContextDescriptor.NoContext;
    private RunnableStep[] _steps = Array.Empty<RunnableStep>();
    private object? _executionContext;

    public IStepResult[] GetResults() => _steps.Select(s => s.Result).ToArray();

    public ScenarioStepsRunner(IRunStageContext parent)
    {
        _parent = parent;
    }

    public ICoreScenarioStepsRunner AddSteps(IEnumerable<StepDescriptor> steps)
    {
        _stepDescriptors = _stepDescriptors.Concat(steps);
        return this;
    }

    public ICoreScenarioStepsRunner WithContext(Func<object> contextProvider, bool takeOwnership)
    {
        _contextDescriptor = new ExecutionContextDescriptor(contextProvider, takeOwnership);
        return this;
    }

    public ICoreScenarioStepsRunner WithContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator>? scopeConfigurator = null)
    {
        _contextDescriptor = new ExecutionContextDescriptor(contextProvider, scopeConfigurator);
        return this;
    }

    public async Task RunAsync()
    {
        _executionContext = CreateExecutionContext();
        PrepareSteps();
        foreach (var step in _steps)
            await step.ExecuteAsync();
    }

    public LightBddConfiguration Configuration => _parent.Engine.Configuration;

    private void PrepareSteps()
    {
        try
        {
            _steps = ProvideSteps(_parent.Info, _stepDescriptors, _executionContext!, _parent.DependencyContainer, string.Empty, _parent.ShouldAbortSubStepExecution);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Scenario steps initialization failed: {e.Message}", e);
        }
        if (_steps.Any(x => x.Result.ExecutionException != null))
            throw new InvalidOperationException("Scenario steps initialization failed.");
    }

    private object CreateExecutionContext()
    {
        try
        {
            return _contextDescriptor.ContextResolver(_parent.DependencyContainer);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Scenario context initialization failed: {e.Message}", e);
        }
    }

    private RunnableStep[] ProvideSteps(IMetadataInfo parent, IEnumerable<StepDescriptor> stepDescriptors, object context, IDependencyContainer container, string groupPrefix, Func<Exception, bool> shouldAbortSubStepExecutionFn)
    {
        var descriptors = stepDescriptors.ToArray();
        if (!descriptors.Any())
            throw new InvalidOperationException("At least one step has to be provided");

        var metadataProvider = _parent.Engine.MetadataProvider;

        var totalSteps = descriptors.Length;
        var steps = new RunnableStep[totalSteps];
        string? previousStepTypeName = null;

        var extensions = _parent.Engine.ExecutionExtensions;
        var stepContext = new RunnableStepContext(_parent.Engine.ExceptionProcessor, _parent.Engine.ProgressNotifier, container, context, ProvideSteps, shouldAbortSubStepExecutionFn, _parent.Engine.ExecutionTimer, _parent.Engine.FileAttachmentsManager);
        for (var stepIndex = 0; stepIndex < totalSteps; ++stepIndex)
        {
            var descriptor = descriptors[stepIndex];
            var stepInfo = new StepInfo(parent, metadataProvider.GetStepName(descriptor, previousStepTypeName), stepIndex + 1, totalSteps, groupPrefix);
            var arguments = descriptor.Parameters.Select(p => new MethodArgument(p, metadataProvider.GetValueFormattingServiceFor(p.ParameterInfo))).ToArray();

            steps[stepIndex] = new RunnableStep(stepContext, stepInfo, descriptor, arguments, extensions.StepDecorators.Concat(metadataProvider.GetStepDecorators(descriptor)));
            previousStepTypeName = stepInfo.Name.StepTypeName?.OriginalName;
        }

        return steps;
    }
}