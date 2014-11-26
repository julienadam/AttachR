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
        public RunResult Run(DebuggingProfile profile, DebuggingTarget singleTarget = null)
        {
            var solutionName = Path.GetFileName(profile.VisualStudioSolutionPath);

            Process visualStudioProcess;
            try
            {
                visualStudioProcess = Retrier.RunWithRetryOnException(
                    () => VisualStudioAttacher.GetVisualStudioForSolution(solutionName));
            }
            catch (AggregateException ex)
            {
                return new RunResult
                {
                    Message = string.Format("Errors while looking for VS.NET instances {0}. Errors : {1}", 
                    profile.VisualStudioSolutionPath,
                    String.Join(". ", ex.InnerExceptions.Select(e => e.Message).ToArray()))
                };
            }
            
            if (visualStudioProcess == null)
            {
                return new RunResult
                {
                    Message = string.Format("No visual studio instance found with solution {0}", profile.VisualStudioSolutionPath)
                };
            }

            profile.CurrentVisualStudioProcess = visualStudioProcess;

            IEnumerable<DebuggingTarget> targets = 
                singleTarget != null
                ? new List<DebuggingTarget> { singleTarget }
                : profile.Targets.ToList();

            foreach (var t in targets.Where(tr => tr.Selected && tr.CurrentProcess == null))
            {
                ProcessStartInfo psi = new ProcessStartInfo(t.Executable, t.CommandLineArguments);
                var process = Process.Start(psi);
                var localTarget = t;
                if (process != null)
                {
                    process.Exited += (sender, args) =>
                    {
                        localTarget.CurrentProcess = null;
                    };

                    try
                    {
                        Retrier.RunWithRetryOnException(() => VisualStudioAttacher.AttachVisualStudioToProcess(visualStudioProcess, process, localTarget.DebuggingEngine.Id));
                    }
                    catch (AggregateException ex)
                    {
                        return new RunResult
                        {
                            Message = string.Format("Errors while attaching process {0} to VS.NET. Errors : {1}",
                            t.Executable,
                            String.Join(". ", ex.InnerExceptions.Select(e => e.Message).ToArray()))
                        };
                    }
                    
                }
                t.CurrentProcess = process;
            }
            return new RunResult();
        }

        public RunResult Stop(DebuggingProfile profile)
        {
            var results = profile.Targets.ToList().Select(t => Stop(t).Message).ToArray();
            return new RunResult
            {
                Message = String.Join(". ", results)
            };
        }

        public RunResult Stop(DebuggingTarget target)
        {
            if (target.CurrentProcess == null)
            {
                return new RunResult
                {
                    Message = string.Format("Process was already killed")
                };
            }

            try
            {
                // Don't ask nicely, just kill the process, we want to be fast here.
                target.CurrentProcess.Kill();
            }
            catch (Exception)
            {
                return new RunResult
                {
                    Message = string.Format("Could not kill process {0}", target.CurrentProcessId)
                };
            }
            
            return new RunResult();
        }

        public void OpenSolution(DebuggingProfile debuggingProfile)
        {
            Process.Start(debuggingProfile.VisualStudioSolutionPath);
        }
    }
}
