using System.ComponentModel;
using AttachR.Components.Recent;
using AttachR.Models;
using AttachR.Serializers;
using Caliburn.Micro;

namespace AttachR.ViewModels
{
    public class MainViewModelWithSampleData : MainViewModel
    {
        public MainViewModelWithSampleData() : base(new RegistryPersister(), new EventAggregator(), new WindowManager(), new PreferencesSerializer())
        {
            DebuggingProfile.VisualStudioSolutionPath = @"C:\Path\To\Some\Solution.sln";
            DebuggingProfile.Targets = new BindingList<DebuggingTargetViewModel>()
            {
                new DebuggingTargetViewModel
                {
                    CommandLineArguments = "Some arguments",
                    Executable = @"C:\Windows\notepad.exe",
                    WorkingDirectory = @"C:\Windows\Temp\",
                    Selected = true
                },
                new DebuggingTargetViewModel
                {
                    CommandLineArguments = "Other arguments",
                },
            };
        }
    }
}