#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation;

internal class StepGroupRunner : ICoreScenarioStepsRunner
{
    private readonly IRunStageContext _parent;
    private readonly string _groupPrefix;
    private IEnumerable<StepDescriptor> _stepDescriptors = Enumerable.Empty<StepDescriptor>();
    private ExecutionContextDescriptor _contextDescriptor = ExecutionContextDescriptor.NoContext;
    private RunnableStepV2[] _steps = Array.Empty<RunnableStepV2>();
    private object? _executionContext;

    public IStepResult[] GetResults() => _steps.Select(s => s.Result).ToArray();

    public StepGroupRunner(IRunStageContext parent, string groupPrefix)
    {
        _parent = parent;
        _groupPrefix = groupPrefix;
    }

    public ICoreScenarioStepsRunner AddSteps(IEnumerable<StepDescriptor> steps)
    {
        _stepDescriptors = _stepDescriptors.Concat(steps);
        return this;
    }

    public ICoreScenarioStepsRunner WithContext(ExecutionContextDescriptor contextDescriptor)
    {
        _contextDescriptor = contextDescriptor;
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
            _steps = ProvideSteps();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Step group initialization failed: {e.Message}", e);
        }
        if (_steps.Any(x => x.Result.ExecutionException != null))
            throw new InvalidOperationException("Step group initialization failed.");
    }

    private object? CreateExecutionContext()
    {
        try
        {
            return _contextDescriptor.ContextResolver(_parent.DependencyContainer);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Step group context initialization failed: {e.Message}", e);
        }
    }

    private RunnableStepV2[] ProvideSteps()
    {
        var descriptors = _stepDescriptors.ToArray();
        if (!descriptors.Any())
            throw new InvalidOperationException("At least one step has to be provided");

        var metadataProvider = _parent.Engine.MetadataProvider;

        var totalSteps = descriptors.Length;
        var steps = new RunnableStepV2[totalSteps];
        string? previousStepTypeName = null;

        var extensions = _parent.Engine.ExecutionExtensions;
        var stepContext = new RunnableStepContextV2(_parent, _executionContext);
        for (var stepIndex = 0; stepIndex < totalSteps; ++stepIndex)
        {
            var descriptor = descriptors[stepIndex];
            var stepInfo = new StepInfo(_parent.Info, metadataProvider.GetStepName(descriptor, previousStepTypeName), stepIndex + 1, totalSteps, _groupPrefix);
            var arguments = descriptor.Parameters.Select(p => new MethodArgument(p, metadataProvider.GetValueFormattingServiceFor(p.ParameterInfo))).ToArray();

            steps[stepIndex] = new RunnableStepV2(stepContext, stepInfo, descriptor, arguments, extensions.StepDecorators.Concat(metadataProvider.GetStepDecorators(descriptor)));
            previousStepTypeName = stepInfo.Name.StepTypeName?.OriginalName;
        }

        return steps;
    }
}