using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Process = System.Diagnostics.Process;

namespace AttachR.VisualStudio
{
    public static class DebuggingEngineDetector
    {
        public static EnvDTE80.Engine[] GetSelectedEngines(IReadOnlyDictionary<string, EnvDTE80.Engine> availableEngines, IEnumerable<string> engines)
        {
            return engines.Select(e => availableEngines[e]).Where(e => e != null).ToArray();
        }

        public static IEnumerable<EnvDTE80.Engine> GetEngines(_DTE dte)
        {
            var debugger = (Debugger2)dte.Debugger;
            var engines = debugger.Transports.Item("Default").Engines;
            for (int i = 0; i < engines.Count; i++)
            {
                yield return engines.Item(i + 1);
            }
        }

        public static DebuggingEngine[] GetAvailableEngines(Process currentVisualStudioProcess, string visualStudioSolutionPath)
        {
            return currentVisualStudioProcess != null 
                ? GetDebuggingEnginesFromVisualStudioInstance(currentVisualStudioProcess)
                : GetDebuggingEnginesForSolution(visualStudioSolutionPath);
        }

        public static DebuggingEngine[] GetDebuggingEnginesForSolution(string visualStudioSolutionPath)
        {
            var vsForSolution = InstanceFinder.GetVisualStudioInstanceForSolution(visualStudioSolutionPath);
            return vsForSolution != null ? GetDebuggingEnginesFromDte(vsForSolution) : new DebuggingEngine[] {};
        }

        public static DebuggingEngine[] GetDebuggingEnginesFromVisualStudioInstance(Process currentVisualStudioProcess)
        {
            _DTE dte;
            return InstanceFinder.TryGetVsInstance(currentVisualStudioProcess.Id, out dte) ? GetDebuggingEnginesFromDte(dte) : new DebuggingEngine[] {};
        }

        private static DebuggingEngine[] GetDebuggingEnginesFromDte(_DTE dte)
        {
            return GetEngines(dte).Select(x => new DebuggingEngine(x.Name, Guid.Parse(x.ID))).ToArray();
        }
    }
}
