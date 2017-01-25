namespace LightBDD.Core.Extensibility
{
    public interface ICoreBddRunner
    {
        IScenarioRunner NewScenario();
    }
}