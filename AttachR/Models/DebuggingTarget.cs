using System;
using System.Collections.Generic;

namespace AttachR.Models
{
    public class DebuggingTarget 
    {
        public bool Selected { get; set; }
        public string Executable { get; set; }
        public string CommandLineArguments { get; set; }
        public string WorkingDirectory { get; set; }
        public TimeSpan Delay { get; set; }
        public List<string> SelectedDebuggingEngines { get; set; }
    }
}
