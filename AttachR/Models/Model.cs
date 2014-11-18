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
        private DebuggingProfile debuggingProfile;
        private string fileName;
        private bool isDirty;

        public DebuggingProfile DebuggingProfile
        {
            get { return debuggingProfile; }
            set
            {
                if (debuggingProfile != value)
                {
                    debuggingProfile = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged();
                }
            } 
        }

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public void Clear()
        {
            DebuggingProfile = new DebuggingProfile();
            Error = "";
            FileName = "";
            IsDirty = false;
        }
    }
}