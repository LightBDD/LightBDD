using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using LightBDD.Core.Formatting;

namespace LightBDD.Core.Extensibility.Implementation
{
	[DebuggerStepThrough]
	internal class NameParser
	{
		private readonly INameFormatter _nameFormatter;

		public NameParser(INameFormatter nameFormatter)
		{
			_nameFormatter = nameFormatter;
		}

		public string GetNameFormat(MethodBase targetMethod, string stepRawName, ParameterDescriptor[] stepParameters)
		{
			var shiftByThisArgument = targetMethod != null && targetMethod.IsDefined(typeof(ExtensionAttribute), true) ? 1 : 0;

			var name = _nameFormatter.FormatName(stepRawName);
			var attributes = targetMethod.GetCustomAttributes(typeof(StepNameAttribute));

			if (attributes != null)
			{
				var stepName = attributes.FirstOrDefault() as StepNameAttribute;

				if (stepName != null)
				{
					if (stepName?.Description != null)
						name = stepName.Description;

					if (!name.StartsWith(stepName?.Name))
						name = stepName.Name + " " + name;
				}
			}

			var sb = new StringBuilder();

			var replacements = stepParameters
					.Skip(shiftByThisArgument)
					.Select((param, index) => ToArgumentReplacement(name, param.RawName, index + shiftByThisArgument))
					.OrderBy(r => r.Position)
					.ThenBy(r => r.Priority);
			var lastPos = 0;
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

		private static ArgumentReplacement ToArgumentReplacement(string stepName, string parameterName, int argumentIndex)
		{
			var position = FindArgument(stepName, parameterName.ToUpperInvariant(), StringComparison.Ordinal);

			if (position >= 0)
				return new ArgumentReplacement(position, $"\"{{{argumentIndex}}}\"", parameterName.Length, 0);

			position = FindArgument(stepName, parameterName, StringComparison.OrdinalIgnoreCase);

			if (position >= 0)
				return new ArgumentReplacement(position + parameterName.Length, $" \"{{{argumentIndex}}}\"", 0, 0);

			return new ArgumentReplacement(stepName.Length, $" [{parameterName}: \"{{{argumentIndex}}}\"]", 0, 1);
		}

		private static int FindArgument(string name, string argument, StringComparison stringComparison)
		{
			var pos = 0;
			while ((pos = name.IndexOf(argument, pos, stringComparison)) >= 0)
			{
				if ((pos == 0 || name[pos - 1] == ' ') &&
						(pos + argument.Length == name.Length || name[pos + argument.Length] == ' '))
					return pos;
				pos += argument.Length;
			}
			return -1;
		}

		private class ArgumentReplacement
		{
			public ArgumentReplacement(int position, string value, int charactersToReplace, byte priority)
			{
				Position = position;
				Value = value;
				CharactersToReplace = charactersToReplace;
				Priority = priority;
			}
			public byte Priority { get; }
			public int Position { get; }
			public string Value { get; }
			public int CharactersToReplace { get; }
		}
	}
}