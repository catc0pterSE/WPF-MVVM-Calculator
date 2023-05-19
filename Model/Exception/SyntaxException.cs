namespace Calculator.Model.Exception;

public class SyntaxException : MathExpressionException
{
    public SyntaxException(string message) : base(message)
    {
    }
}