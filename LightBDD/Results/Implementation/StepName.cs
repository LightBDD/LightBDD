using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LightBDD.Naming;

namespace LightBDD.Results.Implementation
{
    [DebuggerStepThrough]
    internal class StepName : IStepName
    {
        public StepName(string nameFormat) : this(nameFormat, null) { }
        public StepName(string nameFormat, string formattedStepTypeName, params IStepParameter[] parameters)
        {
            Parameters = parameters;
            StepTypeName = formattedStepTypeName;
            NameFormat = nameFormat;
        }

        public string NameFormat { get; private set; }
        public IEnumerable<IStepParameter> Parameters { get; private set; }
        public string StepTypeName { get; private set; }

        public string Format(IStepNameDecorator decorator)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(StepTypeName))
                builder.Append(decorator.DecorateStepTypeName(StepTypeName)).Append(" ");
            builder.AppendFormat(NameFormat, Parameters.Select(decorator.DecorateParameterValue).Cast<object>().ToArray());
            return builder.ToString();
        }
    }
}