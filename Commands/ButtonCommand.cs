using System;
using System.Windows.Input;

namespace Calculator.Commands;

public class ButtonCommand: ICommand
{
    private readonly Action _execute;

    public ButtonCommand(Action execute)
    {
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _execute.Invoke();

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}