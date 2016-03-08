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
            if (aggregator == null) throw new ArgumentNullException(nameof(aggregator));
            if (preferencesSerializer == null) throw new ArgumentNullException(nameof(preferencesSerializer));
            
            this.aggregator = aggregator;
            this.preferencesSerializer = preferencesSerializer;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            var preferences = preferencesSerializer.Load();
            DebugAllShortcut = KeyCombinationConverter.ParseShortcut(preferences.DebugAllShortcut);
            StartAllShortcut = KeyCombinationConverter.ParseShortcut(preferences.StartAllShortcut);
            StopAllShortcut = KeyCombinationConverter.ParseShortcut(preferences.StopAllShortcut);
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

        private KeyCombination stopAllShortcut;
        
        public KeyCombination StopAllShortcut
        {
            get { return stopAllShortcut; }
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
        }
    }
}
