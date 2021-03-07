using System.IO;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Notification.Jsonl.IO;

namespace LightBDD.Reporting.Progressive.UI.Services
{
    public class JsonlImporter
    {
        private readonly EventRepository _repository;

        public JsonlImporter(EventRepository repository)
        {
            _repository = repository;
        }
        public async Task Import(string[] data)
        {
            var serializer = new JsonlEventSerializer();
            int counter = 0;
            foreach (var line in data)
            {
                _repository.Add(serializer.Deserialize(line));
                if (++counter % 100 == 0)
                    await Task.Delay(1);
            }
        }
    }
}
