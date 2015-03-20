using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AttachR.Views;
using Caliburn.Micro;
using Action = System.Action;

namespace AttachR.Components
{
    class SingleInstanceChecker
    {
        private static Mutex mutex;
        const string UniqueEventName = "{969696D0-8E0D-4F8B-B38B-3817FCFC215E}";
        const string UniqueMutexName = "{92270F8C-B338-46B1-B4B3-8D1E684BC57F}";
            
        public bool CheckSingleInstance(SimpleContainer container)
        {
            var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);
            bool isOwned;
            mutex = new Mutex(true, UniqueMutexName, out isOwned);

            if (isOwned)
            {
                // Wait for a signal from another instance
                var thread = new Thread(
                    () =>
                    {
                        while (eventWaitHandle.WaitOne())
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                                (Action)(() =>
                                {
                                    var window = Application.Current.MainWindow;
                                    if (!window.IsVisible)
                                    {
                                        window.Show();
                                    }

                                    if (window.WindowState == WindowState.Minimized)
                                    {
                                        window.WindowState = WindowState.Normal;
                                    }

                                    window.Activate();
                                    try
                                    {
                                        window.Topmost = true;
                                    }
                                    finally
                                    {
                                        window.Topmost = false;
                                    }
                                    window.Focus();
                                }));
                        }
                    });
                thread.IsBackground = true;

                thread.Start();
                return true;
            }

            eventWaitHandle.Set();
            Application.Current.Shutdown();
            return false;
        }
    }
}
