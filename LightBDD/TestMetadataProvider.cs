using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using LightBDD.Formatting.Parameters;
using LightBDD.Naming;

namespace LightBDD
{
    /// <summary>
    /// Test metadata provider allows to retrieve scenario and feature metadata such as descriptions, labels or names.
    /// </summary>
    [DebuggerStepThrough]
    public abstract class TestMetadataProvider
    {
        /// <summary>
        /// Retrieves specified attribute property value.
        /// </summary>
        protected static string ExtractAttributePropertyValue<TAttribute>(MemberInfo member, Func<TAttribute, string> valueExtractor) where TAttribute : Attribute
        {
            return ExtractAttributePropertyValues(member, valueExtractor).SingleOrDefault();
        }

        /// <summary>
        /// Retrieves specified attribute property value for all attribute instances applied on given member.
        /// </summary>
        protected static IEnumerable<string> ExtractAttributePropertyValues<TAttribute>(MemberInfo member, Func<TAttribute, string> valueExtractor) where TAttribute : Attribute
        {
            return ExtractAttributes<TAttribute>(member).Select(valueExtractor);
        }
        /// <summary>
        /// Retrieves specified attributes applied on given member.
        /// </summary>
        protected static IEnumerable<TAttribute> ExtractAttributes<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>();
        }

        /// <summary>
        /// Retrieves feature description from [FeatureDescription] attribute.
        /// If attribute is not defined for feature test class, implementation specific feature description is returned.
        /// </summary>
        /// <returns>Feature description string or null if no description is defined.</returns>
        public string GetFeatureDescription(Type featureTestClass)
        {
            return ExtractAttributePropertyValue<FeatureDescriptionAttribute>(featureTestClass, a => a.Description)
                ?? GetImplementationSpecificFeatureDescription(featureTestClass);
        }

        /// <summary>
        /// Retrieves feature label from [Label] attribute or null if attribute is not defined.
        /// </summary>
        /// <returns>Feature label string or null if label is not defined.</returns>
        public string GetFeatureLabel(Type featureTestClass)
        {
            return ExtractAttributePropertyValue<LabelAttribute>(featureTestClass, a => a.Label);
        }

        /// <summary>
        /// Retrieves feature name which bases on name of feature test class.
        /// </summary>
        /// <returns>Feature name.</returns>
        public string GetFeatureName(Type featureTestClass)
        {
            return NameFormatter.Format(featureTestClass.Name);
        }

        /// <summary>
        /// Retrieves scenario categories from [ScenarioCategory] attribute as well as from implementation specific sources.
        /// </summary>
        /// <returns>Scenario categories</returns>
        public IEnumerable<string> GetScenarioCategories(MethodBase scenarioMethod)
        {
            return ExtractAttributePropertyValues<ScenarioCategoryAttribute>(scenarioMethod, a => a.Name)
                .Concat(ExtractAttributePropertyValues<ScenarioCategoryAttribute>(scenarioMethod.DeclaringType, a => a.Name))
                .Concat(GetImplementationSpecificScenarioCategories(scenarioMethod))
                .Concat(GetImplementationSpecificScenarioCategories(scenarioMethod.DeclaringType))
                .Distinct()
                .OrderBy(c => c);
        }

        /// <summary>
        /// Retrieves scenario label from [Label] attribute or null if attribute is not defined.
        /// </summary>
        /// <returns>Scenario label string or null if label is not defined.</returns>
        public string GetScenarioLabel(MethodBase scenarioMethod)
        {
            return ExtractAttributePropertyValue<LabelAttribute>(scenarioMethod, a => a.Label);
        }

        /// <summary>
        /// Retrieves scenario name which bases on name of scenario method.
        /// </summary>
        /// <returns>Scenario name.</returns>
        public string GetScenarioName(MethodBase scenarioMethod)
        {
            return NameFormatter.Format(scenarioMethod.Name);
        }

        /// <summary>
        /// Returns step name which bases on name of scenario step method.
        /// </summary>
        /// <returns>Step name.</returns>
        public string GetStepName(MethodBase stepMethod)
        {
            return NameFormatter.Format(stepMethod.Name);
        }

        /// <summary>
        /// Returns implementation specific feature description or null if such is not provided.
        /// </summary>
        /// <param name="testClass">Class to analyze.</param>
        /// <returns>Feature description or null.</returns>
        protected abstract string GetImplementationSpecificFeatureDescription(Type testClass);

        /// <summary>
        /// Returns implementation specific scenario categories or empty collection if no categories are provided.
        /// </summary>
        /// <param name="member">Scenario method or feature test class to analyze.</param>
        /// <returns>Scenario categories or empty collection.</returns>
        protected abstract IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member);

        /// <summary>
        /// Returns step name format which bases on name of scenario step method and method parameters.<br/>
        /// If method is parameterized, the step name would contain format parameters {n} that would be replaced with argument values (where {0} refers to first argument) during step execution.<br/>
        /// Please note that rules for placing parameter values in step name are as follows, where first matching rule would be used:
        /// <list type="bullet">
        /// <item><description>it will replace first occurrence of variable name written in capital letters (<c>void Price_is_AMOUNT_dollars(int amount)</c> => <c>Price is "27" dollars</c>)</description></item>
        /// <item><description>it will placed after first occurrence of variable name (<c>void Product_is_in_stock(string product)</c> => <c>Product "desk" is in stock</c>)</description></item>
        /// <item><description>it will placed at the end of step name (<c>void Product_is_in_stock(string productId)</c> => <c>Product is in stock [productId: "ABC123"]</c>)</description></item>
        /// </list>
        /// </summary>
        /// <param name="stepMethod">Step method.</param>
        /// <returns>Step name.</returns>
        public string GetStepNameFormat(MethodInfo stepMethod)
        {
            var name = NameFormatter.Format(stepMethod.Name);
            var sb = new StringBuilder();

            var replacements = stepMethod
                .GetParameters()
                .Select((param, index) => ToArgumentReplacement(name, param, index))
                .OrderBy(r => r.Position)
                .ToArray();
            int lastPos = 0;
            foreach (var replacement in replacements)
            {
                if (lastPos < replacement.Position)
                    sb.Append(name.Substring(lastPos, replacement.Position - lastPos));
                sb.Append(replacement.Value);
                lastPos = replacement.Position + replacement.CharactersToReplace;
            }

            sb.Append(name.Substring(lastPos));
            return sb.ToString();
        }

        /// <summary>
        /// This method is obsoleted.
        /// Please use string GetStepNameFormat(MethodInfo stepMethod)
        /// </summary>
        [Obsolete("Please use string GetStepNameFormat(MethodInfo stepMethod)")]
        public string GetStepNameFormat(string stepType, MethodInfo stepMethod)
        {
            stepType = GetStepTypeName(stepType);

            var format = GetStepNameFormat(stepMethod);
            return string.IsNullOrWhiteSpace(stepType)
            ? format
            : StepNameDecorators.Default.DecorateStepTypeName(stepType) + " " + format;
        }

        /// <summary>
        /// Returns formatted and capitalized step type name or string.Empty if step is meaningless.
        /// </summary>
        public string GetStepTypeName(string stepType)
        {
            return string.IsNullOrWhiteSpace(stepType) ? string.Empty : NameFormatter.Format(stepType).ToUpperInvariant().Trim();
        }

        /// <summary>
        /// Tries to determine a step type from it's name.
        /// If type can be determined, it is returned, while <c>formattedStepName</c> is shortened by the extracted type.
        /// If type cannot be found, or after extraction, the name would be empty or would contain white spaces only, a string.Empty is returned.
        /// 
        /// In order to successful step type determination, the <c>formattedStepName</c> has to start with a string matching any defined step types, followed by space and some non-white characters.
        /// </summary>
        /// <param name="formattedStepName">Formatted step name.</param>
        /// <returns>Step type name or string.Empty</returns>
        public string GetStepTypeNameFromFormattedStepName(ref string formattedStepName)
        {
            var typeNames = new[] { "given", "when", "then", "setup", "and" };
            var name = formattedStepName;

            var type = typeNames
                .Where(n =>
                    name.Length > n.Length &&
                    name.StartsWith(n, StringComparison.OrdinalIgnoreCase) &&
                    name[n.Length] == ' ')
                .Select(GetStepTypeName)
                .FirstOrDefault();

            if (type == null || string.IsNullOrWhiteSpace(name = name.Substring(type.Length + 1)))
                return string.Empty;

            formattedStepName = name;
            return type;
        }

        /// <summary>
        /// Returns step parameter formatter from associated ParameterFormatterAttribute or default one.
        /// </summary>
        public Func<object, string> GetStepParameterFormatter(ParameterInfo parameterInfo)
        {
            Func<object, string> defaultFormatter = o => string.Format("{0}", o);

            var formatters = parameterInfo.GetCustomAttributes(typeof(ParameterFormatterAttribute), true)
                .OfType<ParameterFormatterAttribute>().ToArray();

            if (formatters.Length > 1)
                throw new InvalidOperationException(string.Format(
                    "Parameter can contain only one attribute ParameterFormatterAttribute. Parameter: {0}, Detected attributes: {1}",
                    parameterInfo.Name,
                    string.Join(", ", formatters.Select(f => f.GetType().Name).OrderBy(n => n))));

            return formatters.Length == 1
                ? formatters[0].Format
                : defaultFormatter;
        }

        private static ArgumentReplacement ToArgumentReplacement(string name, ParameterInfo parameterInfo, int argumentIndex)
        {
            string paramName = parameterInfo.Name;
            int position = FindArgument(name, paramName.ToUpperInvariant(), StringComparison.Ordinal);
            if (position >= 0)
                return new ArgumentReplacement(position, string.Format("\"{{{0}}}\"", argumentIndex), paramName.Length);
            position = FindArgument(name, paramName, StringComparison.OrdinalIgnoreCase);
            if (position >= 0)
                return new ArgumentReplacement(position + paramName.Length, string.Format(" \"{{{0}}}\"", argumentIndex), 0);
            return new ArgumentReplacement(name.Length, string.Format(" [{0}: \"{{{1}}}\"]", paramName, argumentIndex), 0);
        }

        private static int FindArgument(string name, string argument, StringComparison stringComparison)
        {
            int pos = 0;
            while ((pos = name.IndexOf(argument, pos, stringComparison)) >= 0)
            {
                if ((pos == 0 || name[pos - 1] == ' ') &&
                    (pos + argument.Length == name.Length || name[pos + argument.Length] == ' '))
                    return pos;
                pos += argument.Length;
            }
            return -1;
        }
    }

    [DebuggerStepThrough]
    internal class ArgumentReplacement
    {
        public ArgumentReplacement(int position, string value, int charactersToReplace)
        {
            Position = position;
            Value = value;
            CharactersToReplace = charactersToReplace;
        }

        public int Position { get; private set; }
        public string Value { get; private set; }
        public int CharactersToReplace { get; private set; }
    }
}