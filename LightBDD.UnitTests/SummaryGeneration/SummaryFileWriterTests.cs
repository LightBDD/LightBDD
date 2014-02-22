using System;
using System.IO;
using System.Linq;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.Results.Implementation;
using LightBDD.SummaryGeneration;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.SummaryGeneration
{
	[TestFixture]
	public class SummaryFileWriterTests
	{
		private SummaryFileWriter _subject;
		private IResultFormatter _formatter;
		private string _filePath;
		private string _dirPath;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_dirPath = Guid.NewGuid().ToString();
			_filePath = string.Format("{0}{1}{2}", _dirPath, Path.DirectorySeparatorChar, Guid.NewGuid().ToString());
			_formatter = MockRepository.GenerateMock<IResultFormatter>();
			_subject = new SummaryFileWriter(_formatter, _filePath);
		}

		#endregion

		[Test]
		public void Should_save_results_to_file_even_if_directory_does_not_exist()
		{
			const string expectedText = "some expected text";
			var feature = new FeatureResult("name", "description", "label");
			try
			{
				_formatter.Stub(f => f.Format(Arg<IFeatureResult[]>.Matches(l => l.Contains(feature)))).Return(expectedText);
				_subject.Save(feature);
				Assert.That(File.ReadAllText(_filePath), Is.EqualTo(expectedText));
			}
			finally
			{
				File.Delete(_filePath);
				Directory.Delete(_dirPath);
			}
		}
	}
}