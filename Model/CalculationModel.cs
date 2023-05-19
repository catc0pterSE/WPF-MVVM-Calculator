using System.Collections.Generic;
using System.Text;
using Calculator.Model.Tokenization;
using Calculator.Model.Tokenization.Tokens;

namespace Calculator.Model;

public class CalculationModel
{
    private readonly Tokenizer _tokenizer = new Tokenizer();
    private readonly ToPostfixConverter _toPostfixConverter = new ToPostfixConverter();
    private readonly PostfixNotationCalculator _calculator = new PostfixNotationCalculator();

    private List<IToken> _infixExpression = new List<IToken>();
    private List<IToken> _postfixExpression = new List<IToken>();
    private IToken? _result = null;

    public void Calculate(string expression)
    {
        Clear();
        _infixExpression = _tokenizer.Parse(expression);
        _postfixExpression = _toPostfixConverter.Convert(_infixExpression);
        _result = _calculator.Calculate(_postfixExpression);
    }

    public void Clear()
    {
        _infixExpression.Clear();
        _postfixExpression.Clear();
        _result = null;
    }

    public string GetInfixExpressionText() =>
        GetExpressionText(_infixExpression);

    public string GetPostfixExpressionText() =>
        GetExpressionText(_postfixExpression);

    public string GetCalculationResultText() =>
        _result != null ? _result.StringValue : "0";

    private string GetExpressionText(List<IToken> expression)
    {
        if (expression.Count == 0)
            return "";
        
        StringBuilder text = new StringBuilder();

        foreach (IToken token in expression)
            text.Append($"{token.StringValue} ");

        return text.ToString();
    }
}