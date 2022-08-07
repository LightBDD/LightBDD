using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.Reporting.UnitTests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FileAttachmentsManager_tests : IDisposable
    {
        private readonly string _directory = Guid.NewGuid().ToString();
        private readonly string _fullDirectoryPath;

        public FileAttachmentsManager_tests()
        {
            _fullDirectoryPath = Path.Combine(AppContext.BaseDirectory, _directory);
        }

        [Test]
        public void Constructor_should_not_create_output_directory()
        {
            var manager = new FileAttachmentsManager(_fullDirectoryPath);
            Assert.That(manager.AttachmentsDirectory, Is.EqualTo(_fullDirectoryPath));
            Assert.That(!Directory.Exists(_fullDirectoryPath), Is.True);
        }

        [Test]
        public async Task Manager_should_support_tilde_and_create_output_directory()
        {
            var manager = new FileAttachmentsManager(Path.Combine("~", _directory));
            await manager.CreateFromText("foo", ".txt", "content", Encoding.UTF8);
            Assert.That(manager.AttachmentsDirectory, Is.EqualTo(_fullDirectoryPath));
            Assert.That(Directory.Exists(_fullDirectoryPath), Is.True);
        }

        [Test]
        public async Task Manager_should_create_attachments_from_byte_content()
        {
            var manager = new FileAttachmentsManager(_fullDirectoryPath);
            var name = Fake.String();
            var content = Fake.String();
            var result = await manager.CreateFromData(name, "TXT", Encoding.UTF8.GetBytes(content));
            AssertAttachment(result, name, content, ".txt");
        }

        [Test]
        public async Task Manager_should_create_attachments_from_text_content()
        {
            var manager = new FileAttachmentsManager(_fullDirectoryPath);
            var name = Fake.String();
            var content = Fake.String();
            var result = await manager.CreateFromText(name, "TXT", content, Encoding.UTF8);
            AssertAttachment(result, name, content, ".txt");
        }

        [Test]
        public async Task Manager_should_create_attachments_from_stream()
        {
            var manager = new FileAttachmentsManager(_fullDirectoryPath);
            var name = Fake.String();
            var content = Fake.String();
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var result = await manager.CreateFromStream(name, ".bin",
                stream => stream.WriteAsync(contentBytes, 0, contentBytes.Length));

            AssertAttachment(result, name, content, ".bin");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Manager_should_create_attachments_from_file(bool shouldMove)
        {
            var manager = new FileAttachmentsManager(_fullDirectoryPath);
            var name = Fake.String();
            var content = Fake.String();
            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, content);

            var result = await manager.CreateFromFile(name, fileName, shouldMove);

            AssertAttachment(result, name, content, Path.GetExtension(fileName));
            Assert.That(File.Exists(fileName), Is.EqualTo(!shouldMove));
        }

        private void AssertAttachment(FileAttachment result, string expectedName, string expectedContent, string expectedExtension)
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(expectedName));
            Assert.That(Path.GetExtension(result.FilePath), Is.EqualTo(expectedExtension));
            Assert.That(Path.GetDirectoryName(result.FilePath), Is.EqualTo(_fullDirectoryPath));
            Assert.That(File.Exists(result.FilePath), Is.True);
            Assert.That(File.ReadAllText(result.FilePath), Is.EqualTo(expectedContent));
        }

        public void Dispose()
        {
            if (Directory.Exists(_fullDirectoryPath))
                Directory.Delete(_fullDirectoryPath, true);
        }
    }
}
