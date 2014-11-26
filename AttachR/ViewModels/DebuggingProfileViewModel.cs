using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AttachR.ViewModels
{
    public class DebuggingProfileViewModel : INotifyPropertyChanged
    {
        public DebuggingProfileViewModel()
        {
            Targets = new BindingList<DebuggingTargetViewModel>();
        }

        [JsonIgnore]
        public Process CurrentVisualStudioProcess { get; set; }

        private BindingList<DebuggingTargetViewModel> targets;
        private Key startHotkey;
        private Key stopHotkey;
        private string visualStudioSolutionPath;

        public string VisualStudioSolutionPath
        {
            get { return visualStudioSolutionPath; }
            set
            {
                if (value == visualStudioSolutionPath) return;
                visualStudioSolutionPath = value;
                OnPropertyChanged();
            }
        }

        public BindingList<DebuggingTargetViewModel> Targets
        {
            get { return targets; }
            set
            {
                if (Equals(value, targets)) return;
                targets = value;
                OnPropertyChanged();
            }
        }

        public Key StartHotkey
        {
            get { return startHotkey; }
            set
            {
                if (value == startHotkey) return;
                startHotkey = value;
                OnPropertyChanged();
            }
        }

        public Key StopHotkey
        {
            get { return stopHotkey; }
            set
            {
                if (value == stopHotkey) return;
                stopHotkey = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}