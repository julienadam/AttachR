using System.Diagnostics;
using System.Linq;
using AttachR.Profiles;

namespace AttachR.Engine
{
    public class Maestro
    {
        public RunResult Run(DebuggingProfile profile)
        {
            var visualStudioProcess = VisualStudioAttacher.GetVisualStudioForSolution(profile.VisualStudioSolutionName);
            if (visualStudioProcess == null)
            {
                return new RunResult { Message = string.Format("No visual studio instance found with solution {0}", profile.VisualStudioSolutionName) };
            }
            profile.CurrentVisualStudioProcess = visualStudioProcess;

            foreach (var target in profile.Targets)
            {
                ProcessStartInfo psi = new ProcessStartInfo(target.Executable, target.CommandLineArguments);
                var process = Process.Start(psi);
                VisualStudioAttacher.AttachVisualStudioToProcess(visualStudioProcess, process);

                target.CurrentProcess = process;
            }
            return new RunResult();
        }

        public void Stop(DebuggingProfile profile)
        {
            profile.Targets.ToList().ForEach(t =>
            {
                if (t.CurrentProcess == null) return;
                
                t.CurrentProcess.Close();
                t.CurrentProcess.Kill();
            });
        }
    }
}
