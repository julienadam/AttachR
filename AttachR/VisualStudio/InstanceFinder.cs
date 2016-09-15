using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using Process = System.Diagnostics.Process;

namespace AttachR.VisualStudio
{
    public static class InstanceFinder
    {
        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        public static IEnumerable<Process> GetVisualStudioProcesses()
        {
            var processes = Process.GetProcesses();
            return processes.Where(o => o.ProcessName.Contains("devenv"));
        }


        private static bool FindVisualStudioInstanceForSolution(string solutionName, out Process vsProcess, out _DTE vsInstance)
        {
            var visualStudios = GetVisualStudioProcesses();

            foreach (var visualStudio in visualStudios)
            {
                _DTE visualStudioInstance;
                if (!TryGetVsInstance(visualStudio.Id, out visualStudioInstance))
                {
                    continue;
                }

                string fullSolutionName;
                try
                {
                    fullSolutionName = visualStudioInstance.Solution.FullName;
                }
                catch (Exception)
                {
                    continue;
                }

                if (string.Compare(new FileInfo(fullSolutionName).FullName, new FileInfo(solutionName).FullName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    vsProcess = visualStudio;
                    vsInstance = visualStudioInstance;
                    return true;
                }
            }

            vsProcess = null;
            vsInstance = null;
            return false;
        }

        public static Process GetVisualStudioProcessForSolution(string solutionName)
        {
            Process process;
            _DTE dte;
            return FindVisualStudioInstanceForSolution(solutionName, out process, out dte) ? process : null;
        }

        public static _DTE GetVisualStudioInstanceForSolution(string solutionName)
        {
            Process process;
            _DTE dte;
            return FindVisualStudioInstanceForSolution(solutionName, out process, out dte) ? dte : null;
        }

        public static bool TryGetVsInstance(int processId, out _DTE instance)
        {
            var numFetched = IntPtr.Zero;
            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            var monikers = new IMoniker[1];
            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();

            while (monikerEnumerator.Next(1, monikers, numFetched) == 0)
            {
                IBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);

                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                if (!runningObjectName.StartsWith("!VisualStudio.DTE"))
                {
                    continue;
                }

                var dte = runningObjectVal as _DTE;

                var split = runningObjectName.Split(':');
                int processIdFromDte;
                if (split.Length == 2 && int.TryParse(split[1], out processIdFromDte) && processIdFromDte == processId)
                {
                    instance = dte;
                    return true;
                }
            }

            instance = null;
            return false;
        }

        public static Process GetAttachedVisualStudioProcess(Process applicationProcess)
        {
            var visualStudios = GetVisualStudioProcesses();

            foreach (var visualStudio in visualStudios)
            {
                _DTE visualStudioInstance;
                if (!TryGetVsInstance(visualStudio.Id, out visualStudioInstance))
                {
                    continue;
                }

                try
                {
                    foreach (var debuggedProcess in visualStudioInstance.Debugger.DebuggedProcesses
                        .Cast<Process>()
                        .Where(debuggedProcess => debuggedProcess.Id == applicationProcess.Id))
                    {
                        return debuggedProcess;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return null;
        }
    }
}
