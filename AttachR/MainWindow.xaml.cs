using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using AttachR.Engine;
using AttachR.Profiles;

namespace AttachR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Maestro maestro = new Maestro();

        private DebuggingProfile DebuggingProfile
        {
            get { return (DebuggingProfile)DataContext; }
            set { DataContext = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            DebuggingProfile = LoadLastProfile();
        }

        private DebuggingProfile LoadLastProfile()
        {
            DebuggingProfile result = new DebuggingProfile
            {
                VisualStudioSolutionName = "RFQHub.sln",
                Targets = new BindingList<DebuggingTarget>
                {
                    new DebuggingTarget
                    {
                        Executable =
                            @"C:\Users\julien.adam\git\rfq-hub\bin\Ids.RfqHub.Client\Debug\Ids.RfqHub.Client.exe",
                        CommandLineArguments =
                            "-shorttitle=true -remoteaddress=https://julienadam.rfq-hub.com:555/RFQHubServer;https://julienadam.rfq-hub.com:556/RFQHubServer -login=sales1@DummyBank1 -password=test -delay=3 -exitnicely=true",
                    }
                }
            };

            return result;
        }

        private void ButtonRun_OnClick(object sender, RoutedEventArgs e)
        {
            maestro.Run(DebuggingProfile);
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            DebuggingProfile.Targets.Add(new DebuggingTarget());
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Open_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
