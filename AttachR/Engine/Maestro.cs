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
        public RunResult Run(string visualStudioSolutionPath, IEnumerable<DebuggingTargetViewModel> debuggingTargets, bool debug, bool runUnselected)
        {
            var targets = debuggingTargets.ToList();
            var requiresVisualStudio = debug && targets.SelectMany(t => t.DebuggingEngines).Any();
            Process visualStudioProcess = null;

            if (requiresVisualStudio)
            {
                try
                {
                    visualStudioProcess = Retrier.RunWithRetryOnException(() => VisualStudioAttacher.GetVisualStudioProcessForSolution(visualStudioSolutionPath));
                }
                catch (AggregateException ex)
                {
                    return new RunResult
                    {
                        Message = $"Errors while looking for VS.NET instances {visualStudioSolutionPath}. Errors : {string.Join(". ", ex.InnerExceptions.Select(e => e.Message).ToArray())}"
                    };
                }

                if (visualStudioProcess == null)
                {
                    return new RunResult
                    {
                        Message = $"No visual studio instance found with solution {visualStudioSolutionPath}"
                    };
                }
            }

            foreach (var target in targets.Where(tr => (runUnselected || tr.Selected) && tr.CurrentProcess == null))
            {
                RunResult run;
                if (!RunSingleTarget(debug, target, visualStudioProcess, out run))
                {
                    return run;
                }
            }

            return new RunResult();
        }

        private bool RunSingleTarget(bool debug, DebuggingTargetViewModel target, Process visualStudioProcess, out RunResult run)
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
                    localTarget.LastError = "";
                    if (engineModes.Any())
                    {
                        var result = Retrier.RunWithRetryOnException(() => VisualStudioAttacher.AttachVisualStudioToProcess(visualStudioProcess, process, !target.UseCustomDebuggingEngines, engineModes));
                        localTarget.LastError = GetError(result);
                    }
                }
                catch (AggregateException ex)
                {
                    localTarget.LastError = ex.Message;

                    {
                        run = new RunResult
                        {
                            Message =
                                $"Errors while attaching process {target.Executable} to VS.NET. Errors : {string.Join(". ", ex.InnerExceptions.Select(e => e.Message).ToArray())}"
                        };
                        return false;
                    }
                }
            }

            target.CurrentProcess = process;
            run = new RunResult();
            return true;
        }

        private string GetError(AttachResult result)
        {
            switch (result)
            {
                case AttachResult.UnknownException:
                    return "Unknown error";
                case AttachResult.NoError:
                    return "";
                case AttachResult.VisualStudioInstanceNotFound:
                    return "Could not find Visual Studio instance";
                case AttachResult.TargetApplicatioNotFound:
                    return "Could not find target application to attach";
                case AttachResult.InvalidEngine:
                    return "Invalid debugging engine, change your selection in the target definition";
                case AttachResult.NoEngineSelected:
                    return "No debugging engine selected";
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        public RunResult Stop(DebuggingProfileViewModel profile)
        {
            var results = profile.Targets.Where(t => t.Selected).ToList().Select(t => Stop(t).Message).ToArray();
            return new RunResult
            {
                Message = string.Join(". ", results)
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
                    Message = $"Could not kill process {target.CurrentProcessId}"
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
