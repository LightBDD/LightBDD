#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility.Implementation;

internal class RunnableScenarioBuilder : IRunnableScenarioBuilder
{
    private readonly EngineContext _context;
    private readonly IFeatureInfo _featureInfo;
    private string? _runtimeId;
    private INameInfo _name = NameInfo.NotSpecified;
    private IReadOnlyList<string> _labels = Array.Empty<string>();
    private IReadOnlyList<string> _categories = Array.Empty<string>();
    private IEnumerable<IScenarioDecorator> _decorators = Array.Empty<IScenarioDecorator>();
    private ScenarioEntryMethod _entryMethod = (_, _) => Task.CompletedTask;

    public RunnableScenarioBuilder(EngineContext context, IFeatureInfo featureInfo)
    {
        _context = context;
        _featureInfo = featureInfo;
    }

    public IRunnableScenarioBuilder WithRuntimeId(string runtimeId)
    {
        _runtimeId = runtimeId;
        return this;
    }

    public IRunnableScenarioBuilder WithName(INameInfo name)
    {
        _name = name;
        return this;
    }

    public IRunnableScenarioBuilder WithLabels(IReadOnlyList<string> labels)
    {
        _labels = labels;
        return this;
    }

    public IRunnableScenarioBuilder WithCategories(IReadOnlyList<string> categories)
    {
        _categories = categories;
        return this;
    }

    public IRunnableScenarioBuilder WithScenarioDecorators(IEnumerable<IScenarioDecorator> scenarioDecorators)
    {
        _decorators = scenarioDecorators;
        return this;
    }

    public IRunnableScenarioBuilder WithScenarioEntryMethod(ScenarioEntryMethod entryMethod)
    {
        _entryMethod = entryMethod;
        return this;
    }

    public IRunnableScenarioV2 Build()
    {
        var scenarioInfo = new ScenarioInfo(_featureInfo, _name, _labels, _categories, _runtimeId);
        var scenarioDecorators = _context.ExecutionExtensions.ScenarioDecorators.Concat(_decorators);
        return new RunnableScenarioV2(_context, scenarioInfo, scenarioDecorators, _entryMethod);
    }
}