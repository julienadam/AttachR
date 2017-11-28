using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using AttachR.Serializers;
using Caliburn.Micro;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AttachR.ViewModels
{
    public class DebuggingTargetViewModel : Screen, ICloneable
    {
        private string executable;
        private string workingDirectory;
        private string commandLineArguments;
        private string lastError;
        private Process currentProcess;
        private ImageSource icon;
        private bool selected;
        private bool useCustomDebuggingEngines;

        public DebuggingTargetViewModel()
        {
            DebuggingEngines = new BindingList<DebuggingEngineViewModel>();
        }

        private DebuggingTargetViewModel([NotNull] BindingList<DebuggingEngineViewModel> engines)
        {
            DebuggingEngines = engines ?? throw new ArgumentNullException(nameof(engines));
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

        public string LastError
        {
            get { return lastError; }
            set
            {
                if (lastError == value)
                {
                    return;
                }
                lastError = value;
                NotifyOfPropertyChange();
            }
        }

        public TimeSpan Delay { get; set; }

        [JsonIgnore]
        public Process CurrentProcess
        {
            get { return currentProcess; }
            set
            {
                if (currentProcess == value)
                {
                    return;
                }
                currentProcess = value;
                NotifyOfPropertyChange(nameof(CurrentProcessId));
                NotifyOfPropertyChange();
            }
        }

        [JsonIgnore]
        public string CurrentProcessId => CurrentProcess?.Id.ToString() ?? "None";

        [NotNull]
        public BindingList<DebuggingEngineViewModel> DebuggingEngines { get; private set; }

        [JsonIgnore]
        public bool UseCustomDebuggingEngines
        {
            get { return useCustomDebuggingEngines; }
            set
            {
                if (value == useCustomDebuggingEngines) return;
                useCustomDebuggingEngines = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonIgnore]
        public List<string> SelectedDebuggingEngines { get; set; }

        public void SetDebuggingEngineSelector(Func<IEnumerable<string>> selector)
        {
            if (SelectedDebuggingEngines == null)
            {
                SelectedDebuggingEngines = new List<string>();
            }

            var availableEngines = selector();
            if (availableEngines == null)
            {
                DebuggingEngines = new BindingList<DebuggingEngineViewModel>();
            }
            else
            {
                var engineListWithSelection = availableEngines.Select(x => new DebuggingEngineViewModel { Name = x, Selected = SelectedDebuggingEngines.Contains(x) }).ToList();
                DebuggingEngines = new BindingList<DebuggingEngineViewModel>(engineListWithSelection);
            }
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
            var list = DebuggingEngines?.Select(e => (DebuggingEngineViewModel) e.Clone()).ToList() ?? new List<DebuggingEngineViewModel>();
            return 
                new DebuggingTargetViewModel(new BindingList<DebuggingEngineViewModel>(list))
                {
                    Delay = Delay,
                    Executable = Executable,
                    WorkingDirectory = WorkingDirectory,
                    LastError = LastError,
                    Icon = Icon,
                    CommandLineArguments = CommandLineArguments,
                    CurrentProcess = CurrentProcess,
                    Selected = Selected,
                    UseCustomDebuggingEngines = UseCustomDebuggingEngines
                };
        }
    }
}