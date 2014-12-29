using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace AttachR.ViewModels
{
    public class DebuggingTargetViewModel : Screen, ICloneable
    {
        private string executable;
        private string workingDirectory;
        private string commandLineArguments;
        private Process currentProcess;
        private ImageSource icon;
        private bool selected;
        
        public DebuggingTargetViewModel()
        {
            debuggingEngines = new BindingList<DebuggingEngineViewModel>(
                ViewModels.DebuggingEngines.AvailableModes
                    .Select(x => new DebuggingEngineViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Selected = false,
                    }).ToList());
        }

        private DebuggingTargetViewModel(BindingList<DebuggingEngineViewModel> engines)
        {
            debuggingEngines = engines;
        }

        [JsonIgnore]
        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                NotifyOfPropertyChange();
            }
        }

        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public string Executable
        {
            get { return executable; }
            set
            {
                if (executable != value)
                {
                    executable = value;
                    Icon = IconLoader.LoadIcon(value);
                    NotifyOfPropertyChange();
                }
            }
        }

        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                if (workingDirectory != value)
                {
                    workingDirectory = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public string CommandLineArguments
        {
            get { return commandLineArguments; }
            set
            {
                if (commandLineArguments != value)
                {
                    commandLineArguments = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public TimeSpan Delay { get; set; }

        [JsonIgnore]
        public Process CurrentProcess
        {
            get { return currentProcess; }
            set
            {
                if (currentProcess != value)
                {
                    currentProcess = value;
                    NotifyOfPropertyChange("CurrentProcessId");
                    NotifyOfPropertyChange();
                }
            }
        }

        [JsonIgnore]
        public string CurrentProcessId
        {
            get { return CurrentProcess != null ? CurrentProcess.Id.ToString() : "None"; }
        }
        
        private readonly BindingList<DebuggingEngineViewModel> debuggingEngines;

        public BindingList<DebuggingEngineViewModel> DebuggingEngines
        {
            get { return debuggingEngines; }
        }

        public void Closing()
        {
            
        }

        public void BrowseForExe(DebuggingTargetViewModel target)
        {
            var executableFile = DialogHelper.ShowOpenDialog("Executable files", "*.exe", target.Executable);
            if (executableFile != null)
            {
                target.Executable = executableFile;
            }
        }

        public void BrowseForDirectory(DebuggingTargetViewModel target)
        {
            var folder = DialogHelper.ShowFolderDialog("Working directory", target.WorkingDirectory);
            if (folder != null)
            {
                target.WorkingDirectory = folder;
            }
        }

        public void Ok()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public object Clone()
        {
            var list = DebuggingEngines.Select(e => (DebuggingEngineViewModel) e.Clone()).ToList();
            return 
                new DebuggingTargetViewModel(new BindingList<DebuggingEngineViewModel>(list))
                {
                    Delay = Delay,
                    Executable = Executable,
                    WorkingDirectory = WorkingDirectory,
                    Icon = Icon,
                    CommandLineArguments = CommandLineArguments,
                    CurrentProcess = CurrentProcess,
                    Selected = Selected
                };
        }
    }
}