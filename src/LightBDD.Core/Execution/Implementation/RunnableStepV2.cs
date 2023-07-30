using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.ExecutionContext.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Execution.Implementation;

internal class RunnableStepV2 : IStep, IRunStageContext
{
    private readonly RunnableStepContextV2 _stepContext;
    private readonly MethodArgument[] _arguments;
    private readonly Func<Task> _decoratedStepMethod;
    private readonly StepResult _result;
    private readonly StepFunc _invocation;
    private readonly ExceptionCollector _exceptionCollector = new();
    public IStepResult Result => _result;
    public EngineContext Engine => _stepContext.Engine;
    IMetadataInfo IRunStageContext.Info => Info;
    public Func<Exception, bool> ShouldAbortSubStepExecution { get; private set; } = _ => true;
    public IDependencyContainer DependencyContainer => _stepContext.Container;
    public IStepInfo Info => Result.Info;
    public IDependencyResolver DependencyResolver => _stepContext.Container;
    public object Context => _stepContext.Context;

    public RunnableStepV2(RunnableStepContextV2 stepContext, StepInfo info, StepDescriptor descriptor, MethodArgument[] arguments, IEnumerable<IStepDecorator> stepDecorators)
    {
        _stepContext = stepContext;
        _invocation = descriptor.StepInvocation;
        _arguments = arguments;
        _decoratedStepMethod = DecoratingExecutor.DecorateStep(this, RunStepAsync, stepDecorators);
        _result = new StepResult(info);
        UpdateNameDetails();
        ValidateDescriptor(descriptor);
    }

    public async Task ExecuteAsync()
    {
        var stepStartNotified = false;
        var executionStartTime = _stepContext.ExecutionTimer.GetTime();
        try
        {
            StartStep(executionStartTime);
            stepStartNotified = true;
            await _decoratedStepMethod.Invoke();
            UpdateStepStatus();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
        finally
        {
            StopStep(executionStartTime, stepStartNotified);
        }
        ProcessExceptions();
    }

    private void ValidateDescriptor(StepDescriptor descriptor)
    {
        if (descriptor.IsValid)
            return;
        HandleException(descriptor.CreationException);
        ProcessExceptions(false);
    }

    private async Task InvokeSubStepsAsync(IStepResultDescriptor result)
    {
        if (result is not CompositeStepResultDescriptor compositeDescriptor)
            return;
        var stepGroupRunner = new StepGroupRunner(this, $"{Info.GroupPrefix}{Info.Number}.");

        try
        {
            await stepGroupRunner
                .AddSteps(compositeDescriptor.SubSteps)
                .WithContext(compositeDescriptor.SubStepsContext)
                .RunAsync();
        }
        finally
        {
            _result.SetSubSteps(stepGroupRunner.GetResults());
        }
    }

    private async Task RunStepAsync()
    {
        IStepResultDescriptor result;
        try
        {
            var args = PrepareArguments();
            try
            {
                result = await AsyncStepSynchronizationContext.Execute(() => _invocation.Invoke(Context, args));
            }
            finally
            {
                CaptureParameterResults();
            }

            VerifyParameterResults();
        }
        catch (Exception e)
        {
            if (ScenarioExecutionException.TryWrap(e, out var wrapped))
                throw wrapped;
            throw;
        }
        finally
        {
            UpdateNameDetails();
        }

        await InvokeSubStepsAsync(result);
    }

    private void VerifyParameterResults()
    {
        var errors = _result.Parameters
            .Where(x => x.Details.VerificationStatus > ParameterVerificationStatus.Success)
            .Select(FormatErrorMessage)
            .ToArray();

        if (!errors.Any())
            return;

        throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
    }

    private void CaptureParameterResults()
    {
        var results = new List<IParameterResult>();
        foreach (var argument in _arguments)
        {
            if (argument.Value is IComplexParameter complex)
                results.Add(new ParameterResult(argument.RawName, complex.Details));
        }

        _result.SetParameters(results);
    }

    private static string FormatErrorMessage(IParameterResult result)
    {
        return $"Parameter '{result.Name}' verification failed: {result.Details.VerificationMessage?.Replace(Environment.NewLine, Environment.NewLine + "\t") ?? string.Empty}";
    }

    private object[] PrepareArguments()
    {
        return _arguments.Select(p => p.Value).ToArray();
    }

    private void UpdateStepStatus()
    {
        _result.UpdateException(null);
        _result.SetStatus(_result.GetSubSteps().GetMostSevereOrNull()?.Status ?? ExecutionStatus.Passed);
    }

    private void ProcessExceptions(bool rethrowIfNeeded = true)
    {
        var exception = _exceptionCollector.CollectFor(_result.Status, _result.GetSubSteps());
        if (exception == null)
            return;

        _result.UpdateException(exception);
        if (rethrowIfNeeded && _stepContext.ShouldAbortSubStepExecution(exception))
            throw new StepExecutionException(exception, _result.Status);
    }

    private void StartStep(EventTime executionStartTime)
    {
        ScenarioExecutionContext.Current.Get<CurrentStepProperty>().Stash(this);
        EvaluateParameters();
        _stepContext.ProgressNotifier.Notify(new StepStarting(executionStartTime, _result.Info));
    }

    private void StopStep(EventTime executionStartTime, bool stepStartNotified)
    {
        ScenarioExecutionContext.Current.Get<CurrentStepProperty>().RemoveCurrent(this);

        var executionStopTime = _stepContext.ExecutionTimer.GetTime();

        _result.SetExecutionTime(executionStopTime.GetExecutionTime(executionStartTime));
        _result.IncludeSubStepDetails();
        if (stepStartNotified)
            _stepContext.ProgressNotifier.Notify(new StepFinished(executionStopTime, _result));
    }

    private void HandleException(Exception exception)
    {
        switch (exception)
        {
            case StepExecutionException e:
                _result.SetStatus(e.StepStatus);
                break;
            case ScenarioExecutionException { InnerException: BypassException }:
                _result.SetStatusV2(ExecutionStatus.Bypassed, exception.InnerException.Message);
                break;
            case ScenarioExecutionException e:
                _stepContext.ExceptionProcessor.UpdateResultsWithException(_result.SetStatusV2, e.InnerException!);
                _exceptionCollector.Capture(e);
                break;
            default:
                _stepContext.ExceptionProcessor.UpdateResultsWithException(_result.SetStatusV2, exception);
                _exceptionCollector.Capture(exception);
                break;
        }
    }

    private void EvaluateParameters()
    {
        foreach (var parameter in _arguments)
            parameter.Evaluate(Context);
        UpdateNameDetails();
    }

    private void UpdateNameDetails()
    {
        if (!_arguments.Any())
            return;

        _result.UpdateName(_arguments.Select(FormatStepParameter).ToArray());
    }

    private INameParameterInfo FormatStepParameter(MethodArgument p)
    {
        try
        {
            return p.FormatNameParameter();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Unable to format '{p.RawName}' parameter of step '{_result.Info}': {e.Message}");
        }
    }

    public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
    {
        ShouldAbortSubStepExecution = shouldAbortExecutionFn ?? throw new ArgumentNullException(nameof(shouldAbortExecutionFn));
    }

    public void Comment(string comment)
    {
        _result.AddComment(comment);
        _stepContext.ProgressNotifier.Notify(new StepCommented(_stepContext.ExecutionTimer.GetTime(), _result.Info, comment));
    }

    public override string ToString()
    {
        return _result.ToString();
    }

    public async Task AttachFile(Func<IFileAttachmentsManager, Task<FileAttachment>> createAttachmentFn)
    {
        var attachment = await createAttachmentFn(_stepContext.FileAttachmentsManager);
        _result.AddAttachment(attachment);
        _stepContext.ProgressNotifier.Notify(new StepFileAttached(_stepContext.ExecutionTimer.GetTime(), _result.Info, attachment));
    }
}