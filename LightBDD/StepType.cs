namespace LightBDD
{
    /// <summary>
    /// Helper type used to indicate lambda parameter that is used to determine type of step (like <c>given</c>, <c>when</c>, <c>then</c>, <c>and</c> etc.).
    /// Used purely by reflection mechanisms.
    /// </summary>
    public sealed class StepType
    {
        internal static readonly StepType Default = new StepType();
    }
}