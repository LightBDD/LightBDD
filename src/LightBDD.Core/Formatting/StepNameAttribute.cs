using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Core.Formatting
{
	public class StepNameAttribute : Attribute
	{
		public StepNameAttribute(string titel, string text)
		{
			Titel = titel;
			Text = text;
		}

		public string Titel { get; }
		public string Text { get; }
	}

	public class GivenAttribute : StepNameAttribute
	{
		public GivenAttribute(string text=null) : base("GIVEN", text)
		{

		}
	}

	public class WhenAttribute : StepNameAttribute
	{
		public WhenAttribute(string text=null) : base("WHEN", text)
		{

		}
	}

	public class ThenAttribute : StepNameAttribute
	{
		public ThenAttribute(string text=null) : base("THEN", text)
		{

		}
	}
}
