using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AttachR.Models
{
    public class Model : INotifyPropertyChanged
    {
        public Model()
        {
            DebuggingProfile = new DebuggingProfile();
        }

        private string error;
        public DebuggingProfile DebuggingProfile { get; set; }
        public string FileName { get; set; }
        public bool IsDirty { get; set; }

        public string Error
        {
            get { return error; }
            set
            {
                if (error != value)
                {
                    error = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}