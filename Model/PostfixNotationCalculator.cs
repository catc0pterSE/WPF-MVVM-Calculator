using System;
using System.Collections.Generic;
using Calculator.Model.Exception;
using Calculator.Model.Tokenization.Tokens;

namespace Calculator.Model;

public class PostfixNotationCalculator
{
    private readonly Stack<OperandToken> _operandTokensStack = new Stack<OperandToken>();

    private readonly Dictionary<OperatorType, Func<(OperandToken left, OperandToken right), OperandToken>>
        _typedBinaryOperations = new Dictionary<OperatorType, Func<(OperandToken left, OperandToken right), OperandToken>>
            {
                [OperatorType.Addition] = arguments => new OperandToken(arguments.left.Value + arguments.right.Value),
                [OperatorType.Subtraction] = arguments => new OperandToken(arguments.left.Value - arguments.right.Value),
                [OperatorType.Multiplication] = arguments => new OperandToken(arguments.left.Value * arguments.right.Value),
                [OperatorType.Division] = arguments => new OperandToken(arguments.left.Value / arguments.right.Value),
                [OperatorType.Power] = arguments => new OperandToken(Math.Pow(arguments.left.Value, arguments.right.Value))
            };

    private readonly Dictionary<OperatorType, Func<OperandToken, OperandToken>> _typedUnaryOperations =
        new Dictionary<OperatorType, Func<OperandToken, OperandToken>>
        {
            [OperatorType.UnaryMinus] = argument => new OperandToken(-argument.Value),
            [OperatorType.SquareRootExtraction] = argument => new OperandToken(Math.Sqrt(argument.Value)),
            [OperatorType.Sine] = argument => new OperandToken(Math.Sin(argument.Value)),
            [OperatorType.Cosine] = argument => new OperandToken(Math.Cos(argument.Value)),
            [OperatorType.Tangent] = argument => new OperandToken(Math.Tan(argument.Value))
        };

    public OperandToken Calculate(IEnumerable<IToken> tokens)
    {
        Clear();
        
        foreach (var token in tokens)
            ProcessToken(token);

        return GetResult();
    }
    
    public void Clear() => 
        _operandTokensStack.Clear();

    private void ProcessToken(IToken token)
    {
        switch (token)
        {
            case OperandToken operandToken:
                StoreOperand(operandToken);
                break;
            case OperatorToken operatorToken:
                ApplyOperator(operatorToken);
                break;
            default:
                throw new SyntaxException($"An unknown token type: {token.GetType()}");
        }
    }

    private OperandToken GetResult()
    {
        if (_operandTokensStack.Count == 0)
            throw new SyntaxException("Check if expression is Empty");

        if (_operandTokensStack.Count != 1)
            throw new SyntaxException("Check if you are providing full expression and the tokens have correct order");

        return _operandTokensStack.Pop();
    }

    private void ApplyOperator(OperatorToken operatorToken)
    {
        OperatorType operatorType = operatorToken.Type;
        OperandToken result;

        if (_typedBinaryOperations.ContainsKey(operatorType))
            result = _typedBinaryOperations[operatorType](GetBinaryOperationArguments());
        else if (_typedUnaryOperations.ContainsKey(operatorType))
            result = _typedUnaryOperations[operatorType](GetUnaryOperationArgument());
        else
            throw new SyntaxException($"An unknown operator type: {operatorToken.Type}");

        _operandTokensStack.Push(result);
    }

    private void StoreOperand(OperandToken operandToken) =>
        _operandTokensStack.Push(operandToken);

    private (OperandToken, OperandToken) GetBinaryOperationArguments()
    {
        if (_operandTokensStack.Count < 2)
            throw new SyntaxException("Not enough arguments to apply binary operation");

        OperandToken right = _operandTokensStack.Pop();
        OperandToken left = _operandTokensStack.Pop();

        return (left, right);
    }

    private OperandToken GetUnaryOperationArgument()
    {
        if (_operandTokensStack.Count < 1)
            throw new SyntaxException("No arguments to apply unary operation");

        return _operandTokensStack.Pop();
    }
}