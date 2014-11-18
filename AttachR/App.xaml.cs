using System.Windows;

namespace AttachR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow w = new MainWindow(e.Args.Length > 0 ? e.Args[0] : null);
            w.Show();
        }
    }
}
