using System;
using System.IO;
using AttachR.Models;
using Newtonsoft.Json;

namespace AttachR.Serializers
{
    public class PreferencesSerializer : IPreferencesSerializer
    {
        private static readonly string preferencesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AttachR");
        private static readonly string path = Path.Combine(preferencesDirectory, "preferences.json");

        public Preferences Load()
        {
            return File.Exists(path) ? JsonConvert.DeserializeObject<Preferences>(File.ReadAllText(path)) : new Preferences();
        }

        public void Save(Preferences model)
        {
            Directory.CreateDirectory(preferencesDirectory);
            File.WriteAllText(path, JsonConvert.SerializeObject(model, Formatting.Indented));
        }
    }
}