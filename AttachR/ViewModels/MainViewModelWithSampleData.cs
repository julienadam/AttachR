using System.ComponentModel;
using AttachR.Components.Recent;
using AttachR.Models;
using Caliburn.Micro;

namespace AttachR.ViewModels
{
    public class MainViewModelWithSampleData : MainViewModel
    {
        public MainViewModelWithSampleData() : base(new RegistryPersister(), new EventAggregator(), new WindowManager())
        {
            DebuggingProfile.VisualStudioSolutionPath = @"C:\Path\To\Some\Solution.sln";
            DebuggingProfile.Targets = new BindingList<DebuggingTargetViewModel>()
            {
                new DebuggingTargetViewModel
                {
                    CommandLineArguments = "Some arguments",
                    Executable = @"C:\Windows\notepad.exe",
                    Selected = true
                },
                new DebuggingTargetViewModel
                {
                    CommandLineArguments = "Other arguments",
                    Executable = @"C:\Windows\System32\Cmd.exe",
                },
            };
        }
    }
}