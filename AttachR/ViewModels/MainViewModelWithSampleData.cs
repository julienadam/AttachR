using System.ComponentModel;
using AttachR.Components.Recent;

namespace AttachR.ViewModels
{
    public class MainViewModelWithSampleData : MainViewModel
    {
        public MainViewModelWithSampleData() : base(new RegistryPersister())
        {
            DebuggingProfile.VisualStudioSolutionPath = @"C:\Path\To\Some\Solution.sln";
            DebuggingProfile.Targets = new BindingList<DebuggingTarget>()
            {
                new DebuggingTarget
                {
                    CommandLineArguments = "Some arguments",
                    Executable = @"C:\Windows\notepad.exe",
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