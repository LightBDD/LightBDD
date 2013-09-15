namespace LightBDD.Naming
{
	/// <summary>
	/// Formats name into readable text.
	/// </summary>
	public static class NameFormatter
	{
		/// <summary>
		/// Formats name into readable text.
		/// This method applies following replacements:<br/>
		/// "__" -> ": "<br/>
		/// "_s_" -> "'s "<br/>
		/// "_" -> " "<br/>
		/// </summary>
		/// <param name="name">Name to format.</param>
		/// <returns>Formatted text.</returns>
		public static string Format(string name)
		{
			return name
				.Replace("__", ": ")
				.Replace("_s_", "'s ")
				.Replace("_", " ");
		}
	}
}