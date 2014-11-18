using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AttachR.Models
{
    public class FileManager
    {
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        };

        public void Save(string path, DebuggingProfile profile)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(profile, settings));
        }

        public DebuggingProfile Open(string filepath)
        {
            return JsonConvert.DeserializeObject<DebuggingProfile>(File.ReadAllText(filepath), settings);
        }
    }
}