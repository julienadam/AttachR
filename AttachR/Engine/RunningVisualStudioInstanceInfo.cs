using EnvDTE;

namespace AttachR.Engine
{
    public class RunningVisualStudioInstanceInfo
    {
        public _DTE Instance { get; }

        public RunningVisualStudioInstanceInfo(_DTE instance)
        {
            Instance = instance;
        }

        public string Name => Instance.Name;
        public string Version => Instance.Version;
        public string Solution => Instance.Solution != null ? Instance.Solution.FullName : "";
    }
}