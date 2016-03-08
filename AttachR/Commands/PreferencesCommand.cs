using System.Windows.Input;

namespace AttachR.Commands
{
    public class PreferencesCommand : RoutedUICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }
    }
}