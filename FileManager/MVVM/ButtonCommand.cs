using System;
using System.Windows.Input;

namespace WpfTest.MVVM
{
    class ButtonCommand : ICommand
    {
        private Action<object?> execute;
        public event EventHandler? CanExecuteChanged;

        public ButtonCommand(Action<object?> execute)
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
