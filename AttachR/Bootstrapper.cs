using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using AttachR.Commands;
using AttachR.Components;
using AttachR.Components.Recent;
using AttachR.ViewModels;
using Caliburn.Micro;
using NHotkey;
using NHotkey.Wpf;

namespace AttachR
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer container = new SimpleContainer();
        
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
            container.Singleton<IEventAggregator, EventAggregator>();
            container.Instance<IRecentFileListPersister>(new RegistryPersister());
            container.Instance<IWindowManager>(new WindowManager());
            container.Singleton<MainViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
            EventHandler<HotkeyEventArgs> runAllHandler = (o, args) =>
            {
                var aggregator = container.GetInstance<IEventAggregator>();
                aggregator.PublishOnUIThread(new RunAllCommand());
            };
            
            EventHandler<HotkeyEventArgs> stopAllHandler = (o, args) =>
            {
                var aggregator = container.GetInstance<IEventAggregator>();
                aggregator.PublishOnUIThread(new StopAllCommand());
            };
            
            HotkeyManager.Current.AddOrReplace("RunAll1", Key.MediaPlayPause, ModifierKeys.Alt | ModifierKeys.Control, runAllHandler);
            HotkeyManager.Current.AddOrReplace("RunAll2", Key.F5, ModifierKeys.Windows, runAllHandler);

            HotkeyManager.Current.AddOrReplace("StopAll1", Key.MediaStop, ModifierKeys.Alt | ModifierKeys.Control, stopAllHandler);
            HotkeyManager.Current.AddOrReplace("StopAll2", Key.F5, ModifierKeys.Windows | ModifierKeys.Shift, stopAllHandler);

            MinimizeToTray.Enable(Application.MainWindow);
        }
    }
}