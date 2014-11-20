using System.Windows.Input;

namespace AttachR.Commands
{
    public class StopAllCommand : RoutedCommand
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