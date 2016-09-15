using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace AttachR.VisualStudio
{
    public class DebuggingEngines
    {
        public Dictionary<AvailableVisualStudioInstall, List<DebuggingEngine>> AvailableDebuggingEnginesByVisualStudioVersion { get; }

        private DebuggingEngines(Dictionary<AvailableVisualStudioInstall, List<DebuggingEngine>> availableDebuggingEnginesByVisualStudioVersion)
        {
            AvailableDebuggingEnginesByVisualStudioVersion = availableDebuggingEnginesByVisualStudioVersion;
        }

        public static DebuggingEngines LoadFromStream(TextReader textReader)
        {
            using (JsonReader reader = new JsonTextReader(textReader))
            {
                var engines = JsonSerializer.Create().Deserialize<Dictionary<AvailableVisualStudioInstall, List<DebuggingEngine>>>(reader);
                return new DebuggingEngines(engines);
            }
        }

        public static DebuggingEngines LoadFromRunningVisualStudios()
        {
            var engines = new Dictionary<AvailableVisualStudioInstall, List<DebuggingEngine>>();

            foreach (var visualStudioProcess in InstanceFinder.GetVisualStudioProcesses())
            {
                var version = FileVersionInfo.GetVersionInfo(visualStudioProcess.MainModule.FileName);
                var install = new AvailableVisualStudioInstall(new Version(version.FileVersion), new FileInfo(visualStudioProcess.MainModule.FileName.ToLowerInvariant()));
                if (engines.ContainsKey(install))
                {
                    continue;
                }

                var enginesForInstance = DebuggingEngineDetector.GetAvailableEngines(visualStudioProcess, null);
                engines.Add(install, enginesForInstance.ToList());
            }

            return new DebuggingEngines(engines);
        }

        public static DebuggingEngines Merge(DebuggingEngines source, DebuggingEngines overrides)
        {
            var result = new Dictionary<AvailableVisualStudioInstall, List<DebuggingEngine>>(source.AvailableDebuggingEnginesByVisualStudioVersion);

            foreach (var enginesByVersion in overrides.AvailableDebuggingEnginesByVisualStudioVersion)
            {
                result[enginesByVersion.Key] = enginesByVersion.Value;
            }

            return new DebuggingEngines(result);
        }
    }
}