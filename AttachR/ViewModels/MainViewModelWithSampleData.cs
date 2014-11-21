using System.ComponentModel;
using AttachR.Components.Recent;
using Caliburn.Micro;

namespace AttachR.ViewModels
{
    public class MainViewModelWithSampleData : MainViewModel
    {
        public MainViewModelWithSampleData() : base(new RegistryPersister(), new EventAggregator())
        {
            DebuggingProfile.VisualStudioSolutionPath = @"C:\Path\To\Some\Solution.sln";
            DebuggingProfile.Targets = new BindingList<DebuggingTarget>()
            {
                new DebuggingTarget
                {
                    CommandLineArguments = "Some arguments",
                    Executable = @"C:\Windows\notepad.exe",
                    Selected = true
                },
                new DebuggingTarget
                {
                    CommandLineArguments = "Other arguments",
                    Executable = @"C:\Windows\System32\Cmd.exe",
                },
            };
        }
    }
}