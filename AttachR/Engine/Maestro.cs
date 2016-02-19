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
        public Func<DebuggingTargetViewModel, IEnumerable<DebuggingEngine>, IEnumerable<DebuggingEngine>> EngineSelectionRequired;

        public Maestro(Func<DebuggingTargetViewModel, IEnumerable<DebuggingEngine>, IEnumerable<DebuggingEngine>> engineSelectionRequired)
        {
            EngineSelectionRequired = engineSelectionRequired;
        }

        public RunResult Run(string visualStudioSolutionPath, IEnumerable<DebuggingTargetViewModel> debuggingTargets, bool debug, bool runUnselected)
        {
            var targets = debuggingTargets.ToList();
            bool requiresVisualStudio = debug && targets.SelectMany(t => t.DebuggingEngines).Any();
            Process visualStudioProcess = null;

            if (requiresVisualStudio)
            {
                try
                {
                    visualStudioProcess = Retrier.RunWithRetryOnException(() => VisualStudioAttacher.GetVisualStudioForSolution(Path.GetFileName(visualStudioSolutionPath)));
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
            }


            foreach (var target in targets.Where(tr => (runUnselected || tr.Selected) && tr.CurrentProcess == null))
            {
                var psi = new ProcessStartInfo(target.Executable, target.CommandLineArguments)
                {
                    WorkingDirectory = target.WorkingDirectory
                };
                var process = Process.Start(psi);
                var localTarget = target;
                if (process != null && debug)
                {
                    process.Exited += (_, __) => localTarget.CurrentProcess = null;

                    try
                    {
                        var engineModes = localTarget.DebuggingEngines.Where(x => x.Selected).Select(x => x.Name).ToArray();
                        if (engineModes.Any())
                        {
                            VisualStudioAttacher.RequestDebuggerSelectionCallback = engines => SelectRequiredEngines(engines.ToList(), localTarget);
                            Retrier.RunWithRetryOnException(() => VisualStudioAttacher.AttachVisualStudioToProcess(visualStudioProcess, process, engineModes));
                        }
                        localTarget.LastError = null;
                    }
                    catch (AggregateException ex)
                    {
                        localTarget.LastError = ex.Message;

                        return new RunResult
                        {
                            Message = string.Format("Errors while attaching process {0} to VS.NET. Errors : {1}",
                            target.Executable,
                            String.Join(". ", ex.InnerExceptions.Select(e => e.Message).ToArray()))
                        };
                    }
                    
                }
                target.CurrentProcess = process;
            }
            return new RunResult();
        }

        private IEnumerable<EnvDTE80.Engine> SelectRequiredEngines(List<EnvDTE80.Engine> available, DebuggingTargetViewModel localTarget)
        {
            var selected = EngineSelectionRequired(localTarget, available.Select(d=> new DebuggingEngine(Guid.Parse(d.ID), d.Name))).Select(e=>e.Id.ToString());
            return available.Where(e=>selected.Contains(e.ID));
        }

        public RunResult Stop(DebuggingProfileViewModel profile)
        {
            var results = profile.Targets.Where(t=>t.Selected).ToList().Select(t => Stop(t).Message).ToArray();
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
                    Message = "Process was already killed"
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
