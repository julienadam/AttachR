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
        public RunResult Run(string visualStudioSolutionPath, IEnumerable<DebuggingTargetViewModel> debuggingTargets, DebuggingTargetViewModel singleTarget = null)
        {
            var solutionName = Path.GetFileName(visualStudioSolutionPath);

            Process visualStudioProcess;
            try
            {
                visualStudioProcess = Retrier.RunWithRetryOnException(() => VisualStudioAttacher.GetVisualStudioForSolution(solutionName));
            }
            catch (AggregateException ex)
            {
                return new RunResult
                {
                    Message = string.Format("Errors while looking for VS.NET instances {0}. Errors : {1}", 
                    visualStudioSolutionPath,
                    String.Join(". ", ex.InnerExceptions.Select(e => e.Message).ToArray()))
                };
            }
            
            if (visualStudioProcess == null)
            {
                return new RunResult
                {
                    Message = string.Format("No visual studio instance found with solution {0}", visualStudioSolutionPath)
                };
            }

            IEnumerable<DebuggingTargetViewModel> targets = 
                singleTarget != null
                ? new List<DebuggingTargetViewModel> { singleTarget }
                : debuggingTargets.ToList();

            foreach (var t in targets.Where(tr => tr.Selected && tr.CurrentProcess == null))
            {
                var psi = new ProcessStartInfo(t.Executable, t.CommandLineArguments)
                {
                    WorkingDirectory = t.WorkingDirectory
                };
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
                        var engineModes = localTarget.DebuggingEngines.Where(x => x.Selected).Select(x => x.Name).ToArray();
                        Retrier.RunWithRetryOnException(() => VisualStudioAttacher.AttachVisualStudioToProcess(visualStudioProcess, process, engineModes));
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

        public RunResult Stop(DebuggingProfileViewModel profile)
        {
            var results = profile.Targets.ToList().Select(t => Stop(t).Message).ToArray();
            return new RunResult
            {
                Message = String.Join(". ", results)
            };
        }

        public RunResult Stop(DebuggingTargetViewModel target)
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

        public void OpenSolution(DebuggingProfileViewModel debuggingProfile)
        {
            Process.Start(debuggingProfile.VisualStudioSolutionPath);
        }
    }
}
