using System;
using System.Windows.Input;

namespace FileManager.MVVM
{
    class RelayCommand : ICommand
    {
        private Action<object?> execute;
        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute)
        {
            this.execute = execute;
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            execute?.Invoke(parameter);
        }
    }
}
