using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AttachR.Profiles
{
    public class DebuggingProfile : INotifyPropertyChanged
    {
        public DebuggingProfile()
        {
            Targets = new BindingList<DebuggingTarget>();
        }

        private string visualStudioSolutionName;
        public string VisualStudioSolutionName
        {
            get { return visualStudioSolutionName; }
            set
            {
                if (visualStudioSolutionName != value)
                {
                    visualStudioSolutionName = value;
                    OnPropertyChanged();
                }
            }
        }

        public BindingList<DebuggingTarget> Targets { get; set; }
        public Key StartHotkey { get; set; }
        public Key StopHotkey { get; set; }
        public Process CurrentVisualStudioProcess { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
