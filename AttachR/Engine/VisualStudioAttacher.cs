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
            if (TryGetVsInstance(visualStudioProcess.Id, out visualStudioInstance))
            {
                try
                {
                    return visualStudioInstance.Solution.FullName;
                }
                catch
                {
                }
            }
            return null;
        }

        public static Process GetAttachedVisualStudio(Process applicationProcess)
        {
            IEnumerable<Process> visualStudios = GetVisualStudioProcesses();

            foreach (Process visualStudio in visualStudios)
            {
                _DTE visualStudioInstance;
                if (!TryGetVsInstance(visualStudio.Id, out visualStudioInstance)) continue;
                try
                {
                    foreach (Process debuggedProcess in visualStudioInstance.Debugger.DebuggedProcesses
                        .Cast<Process>()
                        .Where(debuggedProcess => debuggedProcess.Id == applicationProcess.Id))
                    {
                        return debuggedProcess;
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        private static Dictionary<string, EnvDTE80.Engine> availableEngines;

        public static void AttachVisualStudioToProcess(Process visualStudioProcess, Process applicationProcess, params string[] engines)
        {
            _DTE visualStudioInstance;
            if (!TryGetVsInstance(visualStudioProcess.Id, out visualStudioInstance)) return;

            if (availableEngines == null)
            {
                availableEngines = GetEngines((Debugger2) visualStudioInstance.Debugger).ToDictionary(e => e.Name);
            }

            //Find the process you want the VS instance to attach to...
            EnvDTE.Process processToAttachTo = visualStudioInstance.Debugger.LocalProcesses.Cast<EnvDTE.Process>().FirstOrDefault(process => process.ProcessID == applicationProcess.Id);

            //Attach to the process.
            if (processToAttachTo != null)
            {
                var selectedEngines = engines.Select(e => availableEngines[e]).ToArray();

                if (selectedEngines.Length > 0)
                {
                    var process3 = (EnvDTE90.Process3) processToAttachTo;
                    process3.Attach2(selectedEngines);
                }
                else
                {
                    processToAttachTo.Attach();
                }

                ShowWindow((int)visualStudioProcess.MainWindowHandle, 3);
                SetForegroundWindow(visualStudioProcess.MainWindowHandle);
            }
            else
            {
                throw new InvalidOperationException("Visual Studio process cannot find specified application '" + applicationProcess.Id + "'");
            }
        }

        public static Process GetVisualStudioForSolutions(List<string> solutionNames)
        {
            foreach (string solution in solutionNames)
            {
                Process visualStudioForSolution = GetVisualStudioForSolution(solution);
                if (visualStudioForSolution != null)
                {
                    return visualStudioForSolution;
                }
            }
            return null;
        }

        public static Process GetVisualStudioForSolution(string solutionName)
        {
            IEnumerable<Process> visualStudios = GetVisualStudioProcesses();

            foreach (Process visualStudio in visualStudios)
            {
                _DTE visualStudioInstance;
                if (TryGetVsInstance(visualStudio.Id, out visualStudioInstance))
                {
                    try
                    {
                        string actualSolutionName = Path.GetFileName(visualStudioInstance.Solution.FullName);

                        if (string.Compare(actualSolutionName, solutionName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            return visualStudio;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return null;
        }
        
        private static IEnumerable<Process> GetVisualStudioProcesses()
        {
            Process[] processes = Process.GetProcesses();
            return processes.Where(o => o.ProcessName.Contains("devenv"));
        }

        private static bool TryGetVsInstance(int processId, out _DTE instance)
        {
            IntPtr numFetched = IntPtr.Zero;
            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            IMoniker[] monikers = new IMoniker[1];

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

                var dte = runningObjectVal as _DTE;
                if (dte != null && runningObjectName.StartsWith("!VisualStudio"))
                {
                    int currentProcessId = int.Parse(runningObjectName.Split(':')[1]);

                    if (currentProcessId == processId)
                    {
                        instance = dte;
                        return true;
                    }
                }
            }

            instance = null;
            return false;
        }

        private static IEnumerable<EnvDTE80.Engine> GetEngines(Debugger2 debugger)
        {
            var engines = debugger.Transports.Item("Default").Engines;
            foreach (var x in engines)
            {
                yield return (EnvDTE80.Engine)x;
            }
        }
    }
}