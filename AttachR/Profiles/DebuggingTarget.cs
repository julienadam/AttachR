using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace AttachR.Profiles
{
    public class DebuggingTarget : INotifyPropertyChanged
    {
        private string executable;
        private string commandLineArguments;
        private Process currentProcess;

        public BitmapImage Icon { get; set; }

        public string Executable
        {
            get { return executable; }
            set
            {
                if (executable != value)
                {
                    executable = value;
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

        public Process CurrentProcess
        {
            get { return currentProcess; }
            set
            {
                if (currentProcess != value)
                {
                    currentProcess = value;
                    OnPropertyChanged("CurrentProcessId");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
