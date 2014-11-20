using System.Windows;
using System.Windows.Input;
using AttachR.ViewModels;
using MahApps.Metro.Controls;

namespace AttachR.Views
{
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
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
