using System.Windows;
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
    }
}
