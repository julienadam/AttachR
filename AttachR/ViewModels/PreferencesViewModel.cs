using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AttachR.Commands;
using AttachR.Components.Keyboard;
using AttachR.Models;
using AttachR.Serializers;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AttachR.ViewModels
{
    public class PreferencesViewModel : Screen
    {
        private readonly IEventAggregator aggregator;
        private readonly IPreferencesSerializer preferencesSerializer;

        public PreferencesViewModel([NotNull] IEventAggregator aggregator, [NotNull] IPreferencesSerializer preferencesSerializer)
        {
            this.aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            this.preferencesSerializer = preferencesSerializer ?? throw new ArgumentNullException(nameof(preferencesSerializer));
        }

        protected override void OnViewAttached(object view, object context)
        {
            var preferences = preferencesSerializer.Load();
            DebugAllShortcut = KeyCombinationConverter.ParseShortcut(preferences.DebugAllShortcut);
            StartAllShortcut = KeyCombinationConverter.ParseShortcut(preferences.StartAllShortcut);
            StopAllShortcut = KeyCombinationConverter.ParseShortcut(preferences.StopAllShortcut);

            base.OnViewAttached(view, context);
            // TODO: Unregister hotkeys
        }

        private KeyCombination debugAllShortcut;

        public KeyCombination DebugAllShortcut
        {
            get => debugAllShortcut;
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
            get => startAllShortcut;
            set
            {
                if (Equals(value, startAllShortcut)) return;
                startAllShortcut = value;
                OnPropertyChanged();
            }
        }

        private KeyCombination stopAllShortcut;

        public KeyCombination StopAllShortcut
        {
            get => stopAllShortcut;
            set
            {
                if (Equals(value, stopAllShortcut)) return;
                stopAllShortcut = value;
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
                StartAllShortcut = StartAllShortcut.ToString(),
                StopAllShortcut = StopAllShortcut.ToString()
            });
            TryClose();

            aggregator.PublishOnUIThread(new ReloadPreferencesCommand());
        }

        public void Cancel()
        {
            TryClose();
        }
    }
}
