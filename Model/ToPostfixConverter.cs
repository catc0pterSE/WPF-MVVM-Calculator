using System.Collections.Generic;
using Calculator.Model.Exception;
using Calculator.Model.Tokenization.Tokens;

namespace Calculator.Model;

public class ToPostfixConverter
{
    private readonly Stack<OperatorToken> _tokenStack = new Stack<OperatorToken>();
    private readonly List<IToken> _postfixNotationTokens = new List<IToken>();

    private readonly Dictionary<OperatorType, int> _operatorPriority = new Dictionary<OperatorType, int>()
    {
        [OperatorType.Addition] = 1,
        [OperatorType.Subtraction] = 1,
        [OperatorType.Multiplication] = 2,
        [OperatorType.Division] = 2,
        [OperatorType.Power] = 3,
        [OperatorType.UnaryMinus] = 3,
        [OperatorType.SquareRootExtraction] = 3,
        [OperatorType.Sine] = 3,
        [OperatorType.Cosine] = 3,
        [OperatorType.Tangent] = 3
    };

    private readonly Dictionary<OperatorType, OperatorAssociativity> _operatorAssociativity =
        new Dictionary<OperatorType, OperatorAssociativity>()
        {
            [OperatorType.Addition] = OperatorAssociativity.Left,
            [OperatorType.Subtraction] = OperatorAssociativity.Left,
            [OperatorType.Multiplication] = OperatorAssociativity.Left,
            [OperatorType.Division] = OperatorAssociativity.Left,
            [OperatorType.Power] = OperatorAssociativity.Right,
            [OperatorType.UnaryMinus] = OperatorAssociativity.Right
        };

    public List<IToken> Convert(IEnumerable<IToken> infixNotationTokens)
    {
        foreach (IToken token in infixNotationTokens)
            ProcessToken(token);

        return GetResult();
    }

    
    private List<IToken> GetResult()
    {
        while (_tokenStack.Count > 0)
        {
            OperatorToken operatorToken = _tokenStack.Pop();

            if (operatorToken.Type == OperatorType.OpeningBracket)
                throw new SyntaxException("A redundant opening bracket.");

            _postfixNotationTokens.Add(operatorToken);
        }

        return _postfixNotationTokens;
    }

    private void ProcessToken(IToken token)
    {
        switch (token)
        {
            case OperandToken operandToken:
                StoreOperand(operandToken);
                break;
            case FunctionToken functionToken:
                _tokenStack.Push(functionToken);
                break;
            case OperatorToken operatorToken:
                ProcessOperator(operatorToken);
                break;
            default:
                throw new SyntaxException($"An unknown token type: {token.GetType()}");
        }
    }

    private void ProcessOperator(OperatorToken operatorToken)
    {
        switch (operatorToken.Type)
        {
            case OperatorType.OpeningBracket:
                PushOpeningBracket(operatorToken);
                break;
            case OperatorType.ClosingBracket:
                ProcessBrackets();
                break;
            default:
                PushOperator(operatorToken);
                break;
        }
    }

    private void PushOpeningBracket(OperatorToken operatorToken) =>
        _tokenStack.Push(operatorToken);

    private void ProcessBrackets()
    {
        bool openingBracketFound = false;

        while (_tokenStack.Count > 0)
        {
            OperatorToken stackOperatorToken = _tokenStack.Pop();

            if (stackOperatorToken.Type == OperatorType.OpeningBracket)
            {
                openingBracketFound = true;
                break;
            }

            _postfixNotationTokens.Add(stackOperatorToken);
        }

        if (openingBracketFound == false)
        {
            throw new SyntaxException("An unexpected closing bracket");
        }
    }

    private void PushOperator(OperatorToken operatorToken)
    {
        int operatorTokenPriority = GetOperatorTokenPriority(operatorToken);
        OperatorAssociativity operatorTokenAssociativity = GetOperatorTokenAssociativity(operatorToken);

        while (_tokenStack.Count > 0)
        {
            OperatorToken previousOperatorToken = _tokenStack.Peek();

            if (previousOperatorToken.Type == OperatorType.OpeningBracket)
                break;

            int previousOperatorTokenPriority = GetOperatorTokenPriority(previousOperatorToken);

            if (previousOperatorTokenPriority > operatorTokenPriority ||
                (previousOperatorTokenPriority == operatorTokenPriority &&
                 operatorTokenAssociativity == OperatorAssociativity.Left))
                _postfixNotationTokens.Add(_tokenStack.Pop());
            else
                break;
        }

        _tokenStack.Push(operatorToken);
    }

    private void StoreOperand(OperandToken operandToken)
    {
        _postfixNotationTokens.Add(operandToken);
    }

    private int GetOperatorTokenPriority(OperatorToken operatorToken)
    {
        if (_operatorPriority.ContainsKey(operatorToken.Type) == false)
            throw new SyntaxException($"An unknown operator type: {operatorToken.Type}");

        return _operatorPriority[operatorToken.Type];
    }

    private OperatorAssociativity GetOperatorTokenAssociativity(OperatorToken operatorToken)
    {
        if (_operatorAssociativity.ContainsKey(operatorToken.Type) == false)
            throw new SyntaxException($"An unknown operator type: {operatorToken.Type}");

        return _operatorAssociativity[operatorToken.Type];
    }
}