using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Newtonsoft.Json;

namespace AttachR.Models
{
    public class DebuggingTarget : INotifyPropertyChanged
    {
        private string executable;
        private string commandLineArguments;
        private Process currentProcess;
        private ImageSource icon;

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
        public int? CurrentProcessId
        {
            get { return CurrentProcess != null ? CurrentProcess.Id : (int?) null; }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
