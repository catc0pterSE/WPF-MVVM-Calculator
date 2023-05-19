namespace Calculator.Model.Tokenization.Tokens;

public class OperatorToken : IToken
{
    public OperatorToken(OperatorType operatorType, string stringValue)
    {
        Type = operatorType;
        StringValue = stringValue;
    }
    
    public OperatorType Type { get; }
    public string StringValue { get; }
}