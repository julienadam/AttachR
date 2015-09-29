using System.ComponentModel;
using System.Runtime.CompilerServices;
using AttachR.Components.Keyboard;
using AttachR.Models;
using AttachR.Serializers;
using Caliburn.Micro;

namespace AttachR.ViewModels
{
    public class PreferencesViewModel : Screen
    {
        private readonly PreferencesSerializer preferencesSerializer = new PreferencesSerializer();

        public PreferencesViewModel()
        {
            var preferences = preferencesSerializer.Load();
            DebugAllShortcut = KeyCombinationConverter.ParseShortcut(preferences.DebugAllShortcut);
            StartAllShortcut = KeyCombinationConverter.ParseShortcut(preferences.StartAllShortcut);
        }
        
        private KeyCombination debugAllShortcut;

        public KeyCombination DebugAllShortcut
        {
            get { return debugAllShortcut; }
            set
            {
                if (Equals(value, debugAllShortcut)) return;
                debugAllShortcut = value;
                OnPropertyChanged();
            }
        }

        private KeyCombination startAllShortcut;
        
        public KeyCombination StartAllShortcut
        {
            get { return startAllShortcut; }
            set
            {
                if (Equals(value, startAllShortcut)) return;
                startAllShortcut = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public void Ok()
        {
            preferencesSerializer.Save(new Preferences
            {
                DebugAllShortcut = DebugAllShortcut.ToString(),
                StartAllShortcut = StartAllShortcut.ToString()
            });
            TryClose();
        }

        public void Cancel()
        {
        }
    }
}
