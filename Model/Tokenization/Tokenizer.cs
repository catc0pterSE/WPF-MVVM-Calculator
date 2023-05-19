using System;
using System.Collections.Generic;
using System.Text;
using Calculator.Model.Exception;
using Calculator.Model.Tokenization.Tokens;

namespace Calculator.Model.Tokenization;

public class Tokenizer
{
    private readonly StringBuilder _valueTokenBuilder = new StringBuilder();
    private readonly List<IToken> _infixNotationTokens = new List<IToken>();
    private readonly StringBuilder _functionTokenBuilder = new StringBuilder();

    private readonly Dictionary<char, OperatorType> _operatorTypeMapper = new Dictionary<char, OperatorType>()
    {
        ['+'] = OperatorType.Addition,
        ['-'] = OperatorType.Subtraction,
        ['*'] = OperatorType.Multiplication,
        ['/'] = OperatorType.Division,
        ['^'] = OperatorType.Power,
        ['('] = OperatorType.OpeningBracket,
        [')'] = OperatorType.ClosingBracket,
        ['~'] = OperatorType.UnaryMinus,
    };

    private readonly Dictionary<string, OperatorType> _functionTypeMapper = new Dictionary<string, OperatorType>()
    {
        ["sqr"] = OperatorType.SquareRootExtraction,
        ["sin"] = OperatorType.Sine,
        ["cos"] = OperatorType.Cosine,
        ["tan"] = OperatorType.Tangent
    };

    public List<IToken> Parse(string expression)
    {
        for (int i = 0; i < expression.Length; i++)
        {
            char character = expression[i];

            if (character == '-' && (i == 0 || CanPreсedeUnarySign(expression[i - 1]))) //recognize unary '-''
                character = '~';

            if (IsOperatorCharacter(character))
            {
                if (_functionTokenBuilder.Length > 0)
                    AddFunctionToken();

                if (_valueTokenBuilder.Length > 0)
                    AddOperandToken();

                AddOperatorToken(character);
            }
            else if (Char.IsLetter(character))
            {
                if (_valueTokenBuilder.Length > 0)
                    AddOperandToken();

                _functionTokenBuilder.Append(character);
            }
            else if (Char.IsDigit(character) || IsDecimalSeparator(character))
            {
                if (_functionTokenBuilder.Length > 0)
                    AddFunctionToken();

                _valueTokenBuilder.Append(character);
            }
        }

        return GetResult();
    }


    private List<IToken> GetResult()
    {
        if (_functionTokenBuilder.Length > 0)
            AddFunctionToken();

        if (_valueTokenBuilder.Length > 0)
            AddOperandToken();

        return _infixNotationTokens;
    }

    private void AddOperandToken()
    {
        OperandToken operandToken = CreateOperandToken(_valueTokenBuilder.ToString());
        _valueTokenBuilder.Clear();
        _infixNotationTokens.Add(operandToken);
    }

    private OperandToken CreateOperandToken(string text)
    {
        if (double.TryParse(text, out double result))
            return new OperandToken(result);

        throw new SyntaxException($"Operand {text} has an invalid format");
    }

    private void AddOperatorToken(char character)
    {
        OperatorToken operatorToken = CreateOperatorToken(character);
        _infixNotationTokens.Add(operatorToken);
    }

    private OperatorToken CreateOperatorToken(char character) =>
        new OperatorToken(_operatorTypeMapper[character], character.ToString());

    private void AddFunctionToken()
    {
        string text = _functionTokenBuilder.ToString();
        FunctionToken functionToken = CreateFunctionToken(text);
        _functionTokenBuilder.Clear();
        _infixNotationTokens.Add(functionToken);
    }

    private FunctionToken CreateFunctionToken(string text)
    {
        if (_functionTypeMapper.ContainsKey(text))
            return new FunctionToken(_functionTypeMapper[text], text);

        throw new SyntaxException($"Function {text} has an invalid format");
    }

    private bool CanPreсedeUnarySign(char character) =>
        IsOperatorCharacter(character) && character != ')';

    private bool IsOperatorCharacter(char character) =>
        _operatorTypeMapper.ContainsKey(character);

    private bool IsDecimalSeparator(char character) =>
        character == '.';
}