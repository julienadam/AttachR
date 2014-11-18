using System.IO;
using Newtonsoft.Json;

namespace AttachR.Models
{
    public class FileManager
    {
        public void Save(string path, DebuggingProfile profile)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(profile));
        }

        public DebuggingProfile Open(string filepath)
        {
            return JsonConvert.DeserializeObject<DebuggingProfile>(File.ReadAllText(filepath));
        }
    }
}