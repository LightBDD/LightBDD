namespace LightBDD.Framework;

/// <summary>
/// Enumerable defining scenario priority used to determine execution order.
/// </summary>
public enum ScenarioPriority
{
    /// <summary>
    /// Low priority
    /// </summary>
    Low = -1,
    /// <summary>
    /// Normal priority
    /// </summary>
    Normal = 0,
    /// <summary>
    /// High priority
    /// </summary>
    High = 1
}