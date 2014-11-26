using System.Collections.Generic;
using AttachR.ViewModels;

namespace AttachR.Models
{
    public class DebuggingProfile
    {
        public DebuggingProfile()
        {
            Targets = new List<DebuggingTarget>();
        }

        public string VisualStudioSolutionPath { get; set; }
        public List<DebuggingTarget> Targets { get; set; }
    }
}
