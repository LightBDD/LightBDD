using System;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Exception indicating that step or scenario thrown an exception.
    /// It's purpose is to allow LightBDD engine to process exception and eventually report them back to the underlying test frameworks without exposing LightBDD internal stack frames.
    /// 
    /// The inner exception represents original one that has been thrown by step/scenario.
    /// </summary>
    public class ScenarioExecutionException : Exception
    {
        internal ScenarioExecutionException(Exception inner) : base(string.Empty, inner) { }
    }
}