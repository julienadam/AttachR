using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace AttachR.ViewModels
{
    public static class DebuggingEngines
    {
        private const string StorageFile = "KnownDebuggerEngines";

        public static List<DebuggingEngine> AvailableModes => Load() ?? new List<DebuggingEngine>
        {
            new DebuggingEngine(Guid.NewGuid(), "Debuggers will be selected on the next run"),
        };

        public static void Save(IEnumerable<DebuggingEngine> debuggers)
        {
            using (var file = File.CreateText(StorageFile))
            {
                JsonSerializer
                    .Create()
                    .Serialize(file, debuggers.Reverse());
            }
        }

        private static List<DebuggingEngine> Load()
        {
            try
            {
                using (var file = File.OpenText(StorageFile))
                {
                    return JsonSerializer
                        .Create()
                        .Deserialize<IEnumerable<DebuggingEngine>>(new JsonTextReader(file))
                        .ToList();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}