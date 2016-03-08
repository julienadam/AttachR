using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AttachR.Commands;
using AttachR.Components;
using AttachR.Components.Keyboard;
using AttachR.Components.Recent;
using AttachR.Serializers;
using AttachR.ViewModels;
using Caliburn.Micro;
using NHotkey;
using NHotkey.Wpf;

namespace AttachR
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer container = new SimpleContainer();
        private IEventAggregator aggregator;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return container.GetInstance(serviceType, key) ?? base.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void Configure()
        {
            aggregator = new EventAggregator();
            container.Instance(aggregator);
            container.Instance<IRecentFileListPersister>(new RegistryPersister());
            container.Instance<IWindowManager>(new WindowManager());
            container.Instance<IPreferencesSerializer>(new PreferencesSerializer());
            container.Singleton<MainViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            if (!new SingleInstanceChecker().CheckSingleInstance(container))
            {
                return;
            }

            ApplyPreferences();

            MinimizeToTray.Enable(Application.MainWindow);
        }

        private void ApplyPreferences()
        {
            var s = new PreferencesSerializer();
            var preferences = s.Load();

            DisplayRootViewFor<MainViewModel>();

            RegisterKeyCombination((_, __) => aggregator.PublishOnUIThread(new RunAllCommand()), preferences.StartAllShortcut, "RunAll");
            RegisterKeyCombination((_, __) => aggregator.PublishOnUIThread(new DebugAllCommand()), preferences.DebugAllShortcut, "DebugAll");
            RegisterKeyCombination((_, __) => aggregator.PublishOnUIThread(new StopAllCommand()), preferences.StopAllShortcut, "StopAll");
        }

        private static void RegisterKeyCombination(EventHandler<HotkeyEventArgs> handler, string shortcut, string name)
        {
            var startCombination = KeyCombinationConverter.ParseShortcut(shortcut);
            Key key;
            ModifierKeys modifiers;
            startCombination.CombinationToShortcut(out key, out modifiers);
            TryRegisterHotkey(name, key, modifiers, handler);
        }

        private static void TryRegisterHotkey(string name, Key key, ModifierKeys modifiers, EventHandler<HotkeyEventArgs> handler)
        {
            try
            {
                HotkeyManager.Current.AddOrReplace(name, key, modifiers, handler);
            }
            catch (HotkeyAlreadyRegisteredException)
            {
                // Do not register the hotkey.
            }
        }
    }
}