#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation;

//TODO: refactor
internal class ExecutionStatusCollector
{
    private readonly ConcurrentQueue<Exception> _executionExceptions = new();
    private readonly IExceptionFormatter _exceptionFormatter;

    public ExecutionStatusCollector(IExceptionFormatter exceptionFormatter)
    {
        _exceptionFormatter = exceptionFormatter;
    }

    public void Capture(Exception exception)
    {
        var toCapture = exception is ScenarioExecutionException or StepExecutionException && exception.InnerException != null
            ? exception.InnerException
            : exception;
        _executionExceptions.Enqueue(toCapture);
    }

    public async Task Capture(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            Capture(ex);
        }
    }

    public void Capture(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Capture(ex);
        }
    }

    public void UpdateResults(ScenarioResult result)
    {
        var collected = _executionExceptions.ToArray();
        var status = MapStatus(collected.Select(MapStatus).Concat(result.GetSteps().Select(s => s.Status)));
        var aggregatedException = Aggregate(status, collected, result.GetSteps());
        var details = CollectDetails(status, collected, result.GetSteps());
        result.SetScenarioResult(status, details, aggregatedException);
    }

    private string? CollectDetails(ExecutionStatus status, Exception[] collected, IEnumerable<IStepResult> steps)
    {
        string? message = null;
        if (collected.Any())
            message = $"Scenario {status}: {string.Join(Environment.NewLine, collected.Select(FormatException)).Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")}";

        var details = string.IsNullOrWhiteSpace(message)
            ? Enumerable.Empty<string>()
            : Enumerable.Repeat(message, 1);

        var formattedDetails = string.Join(Environment.NewLine,
            details.Concat(steps.Where(s => !string.IsNullOrWhiteSpace(s.StatusDetails)).Select(s => s.StatusDetails)));
        return string.IsNullOrWhiteSpace(formattedDetails) ? null : formattedDetails;
    }

    private string FormatException(Exception exception)
    {
        return exception is IgnoreException or BypassException
            ? exception.Message
            : _exceptionFormatter.Format(exception);
    }

    private Exception? Aggregate(ExecutionStatus overallStatus, Exception[] collected, IEnumerable<IStepResult> steps)
    {
        var exceptions = collected
            .Concat(steps.Where(s => s.ExecutionException != null).Select(s => s.ExecutionException))
            .Where(ex => MapStatus(ex) >= overallStatus)
            .ToArray();

        return exceptions.Length switch
        {
            0 => null,
            1 => exceptions[0],
            _ => new AggregateException(exceptions)
        };
    }

    private static ExecutionStatus MapStatus(Exception exception)
    {
        return exception switch
        {
            BypassException => ExecutionStatus.Bypassed,
            IgnoreException => ExecutionStatus.Ignored,
            _ => ExecutionStatus.Failed
        };
    }

    private static ExecutionStatus MapStatus(IEnumerable<ExecutionStatus> statuses)
    {
        var result = ExecutionStatus.Passed;
        foreach (var stat in statuses)
        {
            if (stat > result)
                result = stat;
            if (result == ExecutionStatus.Failed)
                break;
        }
        return result;
    }
}