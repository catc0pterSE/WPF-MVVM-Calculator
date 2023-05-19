using System.Globalization;

namespace Calculator.Model.Tokenization.Tokens;

public class OperandToken : IToken
{
    public OperandToken(double value)
    {
        Value = value;
    }

    public double Value { get; private set; }
    public string StringValue => Value.ToString(CultureInfo.CurrentCulture);
}