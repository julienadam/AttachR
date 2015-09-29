using System;
using System.IO;
using AttachR.Models;
using Newtonsoft.Json;

namespace AttachR.Serializers
{
    public class PreferencesSerializer
    {
        private readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AttachR", "preferences.json");

        public Preferences Load()
        {
            return File.Exists(path) ? JsonConvert.DeserializeObject<Preferences>(File.ReadAllText(path)) : new Preferences();
        }

        public void Save(Preferences model)
        {
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AttachR"));
            File.WriteAllText(path, JsonConvert.SerializeObject(model, Formatting.Indented));
        }
    }
}