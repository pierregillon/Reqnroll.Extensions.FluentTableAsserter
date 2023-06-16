using System;
using System.Linq.Expressions;

namespace Specflow.Extensions.FluentTableAsserter;

public record PropertyDefinition<T, TProperty> : IPropertyDefinition<T>
{
    private readonly Type _type;
    private readonly Expression _expression;
    private readonly PropertyConfiguration _configuration;
    private readonly string _memberName;
    private readonly Func<T, TProperty> _func;

    public PropertyDefinition(Type type, Expression<Func<T, TProperty>> expression, PropertyConfiguration configuration)
    {
        _type = type;
        _expression = expression;
        _configuration = configuration;
        _memberName = _configuration.ColumnName ?? FindColumnName(expression);
        _func = expression.Compile();
    }

    public AssertionResult AssertEquivalent(string stringExpectedValue, T data)
    {
        var expectedValue = Convert.ChangeType(stringExpectedValue, _type);
        var actualValue = _func(data);

        if (expectedValue is null && actualValue is null)
        {
            return AssertionResult.Success;
        }

        if (actualValue!.Equals(expectedValue))
        {
            return AssertionResult.Success;
        }

        return AssertionResult.Fail(_memberName, actualValue);
    }

    public bool IsMappedTo(string memberName) => _memberName.EqualsHumanized(memberName);

    private static string FindColumnName(Expression expression)
    {
        string? result;
        try
        {
            result = expression switch
            {
                MemberExpression e => e.Member.Name,
                MethodCallExpression e => e.Method.Name,
                LambdaExpression e => FindColumnName(e.Body),
                UnaryExpression e => FindColumnName(e.Operand),
                _ => null
            };
        }
        catch (InvalidOperationException e)
        {
            throw new InvalidOperationException(
                $"[SpecFlow] did not manage to find a member name for expression {expression}", e);
        }

        if (result == null)
        {
            throw new InvalidOperationException(
                $"[SpecFlow] did not manage to find a member name for expression {expression}");
        }

        return result;
    }
}