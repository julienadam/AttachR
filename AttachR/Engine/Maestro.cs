using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AttachR.ViewModels;

namespace AttachR.Engine
{
    public class Maestro
    {
        public RunResult Run(DebuggingProfile profile, DebuggingTarget target = null)
        {
            var solutionName = Path.GetFileName(profile.VisualStudioSolutionPath);

            var visualStudioProcess = VisualStudioAttacher.GetVisualStudioForSolution(solutionName);
            if (visualStudioProcess == null)
            {
                return new RunResult
                {
                    Message = string.Format("No visual studio instance found with solution {0}", profile.VisualStudioSolutionPath)
                };
            }
            profile.CurrentVisualStudioProcess = visualStudioProcess;

            IEnumerable<DebuggingTarget> targets = 
                target != null
                ? new List<DebuggingTarget> { target }
                : profile.Targets.ToList();

            foreach (var t in targets)
            {
                ProcessStartInfo psi = new ProcessStartInfo(t.Executable, t.CommandLineArguments);
                var process = Process.Start(psi);
                var localTarget = t;
                if (process != null)
                {
                    process.Exited += (sender, args) => { localTarget.CurrentProcess = null; };
                    VisualStudioAttacher.AttachVisualStudioToProcess(visualStudioProcess, process);
                }
                t.CurrentProcess = process;
            }
            return new RunResult();
        }

        public void Stop(DebuggingProfile profile)
        {
            profile.Targets.ToList().ForEach(Stop);
        }

        public void Stop(DebuggingTarget target)
        {
            if (target.CurrentProcess == null)
            {
                return;
            }

            // Don't ask nicely, just kill the process, we want to be fast here.
            target.CurrentProcess.Kill();
        }

        public void OpenSolution(DebuggingProfile debuggingProfile)
        {
            Process.Start(debuggingProfile.VisualStudioSolutionPath);
        }
    }
}
