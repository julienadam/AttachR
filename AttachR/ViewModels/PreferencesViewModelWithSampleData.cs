using System.Windows.Input;
using AttachR.Components.Keyboard;
using AttachR.Serializers;
using Caliburn.Micro;

namespace AttachR.ViewModels
{
    public class PreferencesViewModelWithSampleData : PreferencesViewModel
    {
        public PreferencesViewModelWithSampleData(IEventAggregator aggregator, IPreferencesSerializer serializer) : base(aggregator, serializer)
        {
            DebugAllShortcut = new KeyCombination(Key.LeftCtrl, Key.LeftAlt, Key.MediaPlayPause);
            StartAllShortcut = new KeyCombination(Key.LeftCtrl, Key.LeftAlt, Key.LeftShift, Key.MediaPlayPause);
        }
    }
}