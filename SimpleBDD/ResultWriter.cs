using System;
using System.IO;
using SimpleBDD.Results;
using SimpleBDD.Results.Formatters;

namespace SimpleBDD
{
	/// <summary>
	/// Result writer allowing to save story results to file.
	/// </summary>
	public class ResultWriter
	{
		public XmlResultFormatter ResultFormatter { get; set; }

		public ResultWriter(string testClassName)
		{
			ResultFormatter = new XmlResultFormatter();
			ResultPath = Path.GetFullPath(testClassName + ".xml");
		}
		public string ResultPath { get; private set; }

		public void Save(StoryResult storyResult)
		{
			File.WriteAllText("LoginFeature.xml", ResultFormatter.Format(storyResult));
		}
	}
}