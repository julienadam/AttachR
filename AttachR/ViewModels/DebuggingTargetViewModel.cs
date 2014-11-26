using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace AttachR.ViewModels
{
    public class DebuggingTargetViewModel : Screen, INotifyPropertyChanged
    {
        private string executable;
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

        [JsonIgnore]
        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged("CurrentProcessId");
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        public string CurrentProcessId
        {
            get { return CurrentProcess != null ? CurrentProcess.Id.ToString() : "None"; }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        private readonly BindingList<DebuggingEngineViewModel> debuggingEngines;

        public BindingList<DebuggingEngineViewModel> DebuggingEngines
        {
            get { return debuggingEngines; }
        }

        public void Ok()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}