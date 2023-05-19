using System;
using System.Text;
using Calculator.Commands;
using Calculator.Model;

namespace Calculator.ViewModel;

public class CalculatorViewModel : ViewModelBase
{
    private readonly CalculationModel _calculationModel = new CalculationModel();
    private StringBuilder _input = new StringBuilder();
    private string _errorMessage;
    private InputButtonCommand<string>? _inputButtonCommand;
    private ButtonCommand _clearButtonCommand;
    private ButtonCommand _calculateButtonCommand;

    public string Input
    {
        get => _input.ToString();
        set
        {
            _input = new StringBuilder(value);
            OnPropertyChanged();
        }
    }
    
    public string Infix => _calculationModel.GetInfixExpressionText();
    public string Postfix => _calculationModel.GetPostfixExpressionText();
    public string Result => _calculationModel.GetCalculationResultText();
    public string Message => _errorMessage;

    public InputButtonCommand<string> InputButtonCommand =>
        _inputButtonCommand ??= new InputButtonCommand<string>(OnInputButtonPressed);

    public ButtonCommand ClearButtonCommand =>
        _clearButtonCommand ??= new ButtonCommand(OnClearButtonPressed);

    public ButtonCommand CalculateButtonCommand =>
        _calculateButtonCommand ??= new ButtonCommand(OnCalculateButtonPressed);

    private void OnInputButtonPressed(string digit)
    {
        _input.Append(digit);
        OnPropertyChanged(nameof(Input));
    }
    
    private void OnClearButtonPressed()
    {
        _input.Clear();
        _calculationModel.Clear();
        _errorMessage = "";
        UpdateView();
    }
    
    private void OnCalculateButtonPressed()
    {
        try
        {
            _calculationModel.Calculate(Input);
        }
        catch (Exception exception)
        {
            _calculationModel.Clear();
            _errorMessage = exception.Message;
        }
        
        UpdateView();
    }

    private void UpdateView()
    {
        OnPropertyChanged(nameof(Input));
        OnPropertyChanged(nameof(Infix));
        OnPropertyChanged(nameof(Postfix));
        OnPropertyChanged(nameof(Result));
        OnPropertyChanged(nameof(Message));
    }
    
  
}