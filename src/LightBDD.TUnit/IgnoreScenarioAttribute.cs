namespace LightBDD.TUnit
{
    /// <summary>
    /// Attribute allowing to ignore scenario in declarative way. It can be applied on scenario method or step method as well as feature class.
    /// If applied on scenario, no steps will be executed, but scenario will be included in reports.
    /// If applied on class level, all scenarios in this class will get ignored.
    /// It is recommended to use this attribute in favor of <see cref="SkipAttribute"/>.
    /// </summary>
    public class IgnoreScenarioAttribute : SkipAttribute
    {
        /// <summary>
        /// Default constructor allowing to specify ignore reason.
        /// </summary>
        /// <param name="reason">Ignore reason.</param>
        public IgnoreScenarioAttribute(string reason) : base(reason)
        {
        }
        
        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// </summary>
        public new int Order { get; set; }
    }
}