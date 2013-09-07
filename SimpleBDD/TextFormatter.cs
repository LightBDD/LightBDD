namespace SimpleBDD
{
	public static class TextFormatter
	{
		public static string Format(string name)
		{
			return name
				.Replace("__", ": ")
				.Replace("_s_", "'s ")
				.Replace("_", " ");
		}
	}
}