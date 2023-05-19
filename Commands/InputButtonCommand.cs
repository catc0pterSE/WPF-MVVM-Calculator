using System;
using System.Windows.Input;

namespace Calculator.Commands;

public class InputButtonCommand<T> : ICommand
{
    private Action<T> _execute;
    private Func<T, bool>? _canExecute;

    public InputButtonCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        if (parameter is T tParameter)
            return _canExecute == null || _canExecute.Invoke(tParameter);

        return false;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T tParameter)
            _execute.Invoke(tParameter);
    }
}