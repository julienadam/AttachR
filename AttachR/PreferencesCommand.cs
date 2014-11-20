using System.Windows.Input;

namespace AttachR
{
    public class PreferencesCommand : RoutedUICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //PreferencesWindow window = new PreferencesWindow();
            //window.ShowDialog();
        }
    }
}