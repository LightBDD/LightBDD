#nullable enable
using System.Threading.Tasks;

namespace LightBDD.Core.Extensibility;

/// <summary>
/// Scenario entry point.
/// </summary>
/// <param name="fixture">Scenario feature fixture</param>
/// <param name="coreScenarioRunner">Scenario steps builder</param>
/// <returns></returns>
public delegate Task ScenarioEntryMethod(object fixture, ICoreScenarioBuilderV2 coreScenarioRunner);