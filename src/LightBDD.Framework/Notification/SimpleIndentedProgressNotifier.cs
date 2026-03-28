using System;

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// A progress notifier that produces clean, YAML-like indented output.
    /// <br/>
    /// This notifier inherits from <see cref="DefaultProgressNotifier"/> and pre-configures it with:
    /// <list type="bullet">
    /// <item><description>Hierarchical indentation for sub-steps (<see cref="DefaultProgressNotifier.IndentLength"/> = 4).</description></item>
    /// <item><description>Step numbering as a suffix (<see cref="DefaultProgressNotifier.StepWordAndStepNumberOnStart"/> and <see cref="DefaultProgressNotifier.StepWordAndStepNumberOnFinish"/> = <see cref="StepWordAndStepNumberBehaviour.IncludeAsSuffix"/>).</description></item>
    /// <item><description>Suppressed finish notifications for passing steps (<see cref="DefaultProgressNotifier.WriteSuccessMessageForBasicSteps"/> = false).</description></item>
    /// <item><description>Blank lines around scenario and feature boundaries for visual separation.</description></item>
    /// </list>
    /// </summary>
    public class SimpleIndentedProgressNotifier : DefaultProgressNotifier
    {
        /// <summary>
        /// Initializes the notifier with <paramref name="onNotify"/> actions that will be used to delegate the rendered notification text.
        /// </summary>
        /// <param name="onNotify">One or more actions to receive notification text.</param>
        public SimpleIndentedProgressNotifier(params Action<string>[] onNotify)
            : base(onNotify)
        {
            WriteSuccessMessageForBasicSteps = false;
            ShowFinalStepWithEachStep = false;
            IndentLength = 4;
            StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            StepWordAndStepNumberOnFinish = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            IncludeStepNameOnFinish = false;
            IncludeEllipsisAfterStep = false;
            WriteBlankLineAfterFeatureStart = true;
            WriteBlankLineAfterScenarioStart = true;
            WriteBlankLinesAroundScenarioFinish = true;
        }
    }
}
