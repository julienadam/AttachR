using System.Windows;
using System.Windows.Input;
using AttachR.ViewModels;

namespace AttachR.Views
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            StopAll.ToolTip = "Stop all the executables\r\nCTRL+ALT+STOP\r\nWIN+SHIFT+F5";
            RunAll.ToolTip = "Run all the executables\r\nCTRL+ALT+PLAY\r\nWIN+F5";
        }

        private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
            RecentFileList.Persister = ((MainViewModel) DataContext).Persister;
        }

        private void CommandBinding_RunAll_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((MainViewModel) DataContext).RunAll();
        }

        private void CommandBinding_StopAll_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((MainViewModel)DataContext).StopAll();
        }
    }

    
}
