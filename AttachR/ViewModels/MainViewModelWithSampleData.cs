using System.ComponentModel;
using AttachR.Components.Recent;
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
                    CommandLineArguments = "Some really long arguments that will hopefully trigger the resize bug on the target parameters control that forces the control to the size of the text instead of having a size relative to the size of the window",
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