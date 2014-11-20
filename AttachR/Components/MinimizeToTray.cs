using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace AttachR.Components
{
    /// <summary>
    /// Class implementing support for "minimize to tray" functionality.
    /// </summary>
    public static class MinimizeToTray
    {
        /// <summary>
        /// Enables "minimize to tray" behavior for the specified Window.
        /// </summary>
        /// <param name="window">Window to enable the behavior for.</param>
        public static void Enable(Window window)
        {
            // No need to track this instance; its event handlers will keep it alive
            // ReSharper disable once ObjectCreationAsStatement
            new MinimizeToTrayInstance(window);
        }

        /// <summary>
        /// Class implementing "minimize to tray" functionality for a Window instance.
        /// </summary>
        private class MinimizeToTrayInstance
        {
            private readonly Window window;
            private TaskbarIcon notifyIcon;
        
            /// <summary>
            /// Initializes a new instance of the MinimizeToTrayInstance class.
            /// </summary>
            /// <param name="window">Window instance to attach to.</param>
            public MinimizeToTrayInstance(Window window)
            {
                Debug.Assert(window != null, "window parameter is null.");
                this.window = window;
                this.window.StateChanged += HandleStateChanged;
            }

            /// <summary>
            /// Handles the Window's StateChanged event.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">Event arguments.</param>
            private void HandleStateChanged(object sender, EventArgs e)
            {
                if (notifyIcon == null)
                {
                    // Initialize NotifyIcon instance "on demand"
                    notifyIcon = new TaskbarIcon
                    {
                        Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location),
                        DoubleClickCommand = new ShowWindowCommand(window)
                    };
                }
            
                // Update copy of Window Title in case it has changed
                notifyIcon.ToolTip = window.Title;

                // Show/hide Window and NotifyIcon
                var minimized = (window.WindowState == WindowState.Minimized);
                window.ShowInTaskbar = !minimized;
                notifyIcon.Visibility = minimized ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public class ShowWindowCommand : ICommand
        {
            private readonly Window window;

            public ShowWindowCommand(Window window)
            {
                this.window = window;
            }

            public bool CanExecute(object parameter)
            {
                return window.WindowState != WindowState.Normal;
            }

            public void Execute(object parameter)
            {
                window.WindowState = WindowState.Normal;
            }

            public event EventHandler CanExecuteChanged;
        }
    }

    
}