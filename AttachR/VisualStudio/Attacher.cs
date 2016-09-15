using System;
using System.Linq;
using System.Runtime.InteropServices;
using AttachR.Engine;
using EnvDTE;
using Process = System.Diagnostics.Process;

namespace AttachR.VisualStudio
{
    public static class Attacher
    {
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);


        public static AttachResult AttachVisualStudioToProcess(Process visualStudioProcess, Process applicationProcess, bool customDebuggingEngines, params string[] engines)
        {
            _DTE visualStudioInstance;
            if (!InstanceFinder.TryGetVsInstance(visualStudioProcess.Id, out visualStudioInstance))
            {
                return AttachResult.VisualStudioInstanceNotFound;
            }

            var availableEngines = DebuggingEngineDetector.GetEngines(visualStudioInstance).ToDictionary(e => e.Name);

            //Find the process you want the VS instance to attach to...
            var processToAttachTo = visualStudioInstance.Debugger.LocalProcesses.Cast<EnvDTE.Process>().FirstOrDefault(process => process.ProcessID == applicationProcess.Id);

            //Attach to the process.
            if (processToAttachTo == null)
            {
                return AttachResult.TargetApplicatioNotFound;
            }

            if (customDebuggingEngines)
            {
                var selectedEngines = DebuggingEngineDetector.GetSelectedEngines(availableEngines, engines);
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

    }
}