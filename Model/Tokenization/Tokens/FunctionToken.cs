namespace Calculator.Model.Tokenization.Tokens;

public class FunctionToken: OperatorToken
{
    public FunctionToken(OperatorType operatorType, string stringValue) : base(operatorType, stringValue)
    {
    }
}