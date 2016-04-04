using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using EnvDTE80;
using Process = System.Diagnostics.Process;

namespace AttachR.Engine
{
    /// <summary>
    /// I found this gem originally on PasteBin (http://pastebin.com/pHWMP3EQ) and (http://pastebin.com/KKyBpWQW).
    /// Since it was dropped there anonymously, it had little documentation, and I made a change, I thought I'd migrate it to a gist.
    /// I found this snippet of code extremely useful, it flows much better than the alternative Debugger.Launch() call.
    /// </summary>
    public static class VisualStudioAttacher
    {
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);


        public static string GetSolutionForVisualStudio(Process visualStudioProcess)
        {
            _DTE visualStudioInstance;
            if (!TryGetVsInstance(visualStudioProcess.Id, out visualStudioInstance))
            {
                return null;
            }

            try
            {
                return visualStudioInstance.Solution.FullName;
            }
            catch
            {
                return null;
            }
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

        public static AttachResult AttachVisualStudioToProcess(Process visualStudioProcess, Process applicationProcess, bool customDebuggingEngines, params string[] engines)
        {
            _DTE visualStudioInstance;
            if (!TryGetVsInstance(visualStudioProcess.Id, out visualStudioInstance))
            {
                return AttachResult.VisualStudioInstanceNotFound;
            }

            var availableEngines = GetEngines(visualStudioInstance).ToDictionary(e => e.Name);

            //Find the process you want the VS instance to attach to...
            var processToAttachTo = visualStudioInstance.Debugger.LocalProcesses.Cast<EnvDTE.Process>().FirstOrDefault(process => process.ProcessID == applicationProcess.Id);

            //Attach to the process.
            if (processToAttachTo == null)
            {
                return AttachResult.TargetApplicatioNotFound;
            }

            if (customDebuggingEngines)
            {
                var selectedEngines = GetSelectedEngines(availableEngines, engines);
                if (selectedEngines.Length == 0)
                {
                    return AttachResult.NoEngineSelected;
                }

                try
                {
                    var process3 = (EnvDTE90.Process3)processToAttachTo;
                    process3.Attach2(selectedEngines);
                }
                catch (COMException ex)
                {
                    return ex.HResult == -1989083106 ? AttachResult.InvalidEngine : AttachResult.UnknownException;
                }
            }
            else
            {
                try
                {
                    processToAttachTo.Attach();
                }
                catch (COMException)
                {
                    return AttachResult.UnknownException;
                }
            }

            ShowWindow((int)visualStudioProcess.MainWindowHandle, 3);
            SetForegroundWindow(visualStudioProcess.MainWindowHandle);

            return AttachResult.NoError;
        }

        private static EnvDTE80.Engine[] GetSelectedEngines(IReadOnlyDictionary<string, EnvDTE80.Engine> availableEngines, IEnumerable<string> engines)
        {
            return engines.Select(e => availableEngines[e]).Where(e => e != null).ToArray();
        }

        public static Process GetVisualStudioForSolutions(List<string> solutionNames)
        {
            foreach (var solution in solutionNames)
            {
                var visualStudioForSolution = GetVisualStudioProcessForSolution(solution);
                if (visualStudioForSolution != null)
                {
                    return visualStudioForSolution;
                }
            }
            return null;
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

        private static IEnumerable<Process> GetVisualStudioProcesses()
        {
            var processes = Process.GetProcesses();
            return processes.Where(o => o.ProcessName.Contains("devenv"));
        }

        private static bool TryGetVsInstance(int processId, out _DTE instance)
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

        public static IEnumerable<EnvDTE80.Engine> GetEngines(_DTE dte)
        {
            var debugger = (Debugger2)dte.Debugger;
            var engines = debugger.Transports.Item("Default").Engines;
            for (int i = 0; i < engines.Count; i++)
            {
                yield return engines.Item(i+1);
            }
        }

        public static string[] GetAvailableEngines(Process currentVisualStudioProcess, string visualStudioSolutionPath)
        {
            if (currentVisualStudioProcess != null)
            {
                _DTE dte;
                if (TryGetVsInstance(currentVisualStudioProcess.Id, out dte))
                {
                    var engines = GetEngines(dte);
                    return engines.Select(x => x.Name).ToArray();
                }
            }
            else
            {
                var vsForSolution = GetVisualStudioInstanceForSolution(visualStudioSolutionPath);
                if (vsForSolution != null)
                {
                    var engines = GetEngines(vsForSolution);
                    return engines.Select(x => x.Name).ToArray();
                }
            }

            return null;
        }
    }
}