using System.Windows.Input;
using AttachR.Components.Keyboard;

namespace AttachR.ViewModels
{
    public class PreferencesViewModelWithSampleData : PreferencesViewModel
    {
        public PreferencesViewModelWithSampleData()
        {
            DebugAllShortcut = new KeyCombination(Key.LeftCtrl, Key.LeftAlt, Key.MediaPlayPause);
            StartAllShortcut = new KeyCombination(Key.LeftCtrl, Key.LeftAlt, Key.LeftShift, Key.MediaPlayPause);
        }
    }
}