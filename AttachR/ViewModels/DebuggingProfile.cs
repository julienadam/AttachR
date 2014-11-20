using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Newtonsoft.Json;

namespace AttachR.ViewModels
{
    public class DebuggingProfile : INotifyPropertyChanged
    {
        public DebuggingProfile()
        {
            Targets = new BindingList<DebuggingTarget>();
        }

        private string visualStudioSolutionPath;
        public string VisualStudioSolutionPath
        {
            get { return visualStudioSolutionPath; }
            set
            {
                if (visualStudioSolutionPath != value)
                {
                    visualStudioSolutionPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public BindingList<DebuggingTarget> Targets { get; set; }
        public Key StartHotkey { get; set; }
        public Key StopHotkey { get; set; }
        
        [JsonIgnore]
        public Process CurrentVisualStudioProcess { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
