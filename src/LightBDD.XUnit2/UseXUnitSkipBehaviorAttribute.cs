using System;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// Attribute allowing to control the xunit Fact, Theory, Scenario and InlineData skip behavior.<br/>
    /// When applied on assembly, all Scenarios and InlineData attributes with Skip property set will result with scenario being immediately ignored without running them.<br/>
    /// When not applied, scenarios will always run but LightBDD will ignore them just before running any steps, which will allow to include those scenarios in the LightBDD results.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class UseXUnitSkipBehaviorAttribute : Attribute
    {
    }
}