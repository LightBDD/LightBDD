using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
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
            return member.GetCustomAttributes(typeof(TAttribute), true)
                .OfType<TAttribute>()
                .Select(valueExtractor)
                .SingleOrDefault();
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
        /// Returns step name format which starts with formatted and capitalized <c>stepType</c> and bases on name of scenario step method.<br/>
        /// If method is parameterized, the step name would contain format parameters {n} that would be replaced with argument values during step execution.<br/>
        /// Please note that rules for placing parameter values in step name are as follows, where first matching rule would be used:
        /// <list type="bullet">
        /// <item><description>it will replace first occurrence of variable name written in capital letters (<c>void Price_is_AMOUNT_dollars(int amount)</c> => <c>Price is "27" dollars</c>)</description></item>
        /// <item><description>it will placed after first occurrence of variable name (<c>void Product_is_in_stock(string product)</c> => <c>Product "desk" is in stock</c>)</description></item>
        /// <item><description>it will placed at the end of step name (<c>void Product_is_in_stock(string productId)</c> => <c>Product is in stock [productId: "ABC123"]</c>)</description></item>
        /// </list>
        /// </summary>
        /// <param name="stepType">Step type like given, when, then, and etc.</param>
        /// <param name="stepMethod">Step method.</param>
        /// <returns>Step name.</returns>
        public string GetStepNameFormat(string stepType, MethodInfo stepMethod)
        {
            var name = NameFormatter.Format(stepMethod.Name);
            var sb = new StringBuilder();

            var stepTypeName = NameFormatter.Format(stepType).ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(stepTypeName))
                sb.Append(stepTypeName).Append(' ');

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