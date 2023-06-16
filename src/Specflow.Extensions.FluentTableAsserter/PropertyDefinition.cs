using System;
using System.Linq.Expressions;

namespace Specflow.Extensions.FluentTableAsserter;

public record PropertyDefinition<T, TProperty>(
    Expression<Func<T, TProperty>> Expression,
    PropertyConfiguration Configuration
) : IPropertyDefinition<T>
{
    private readonly string _memberName = FindColumnName(Expression);
    private readonly Func<T, TProperty> _func = Expression.Compile();

    private string ColumnOrMemberName => Configuration.ColumnName ?? _memberName;

    public AssertionResult AssertEquivalent(string stringExpectedValue, T data)
    {
        var expectedValue = ConvertToPropertyType(stringExpectedValue);
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

    private object ConvertToPropertyType(string stringExpectedValue)
    {
        try
        {
            return Convert.ChangeType(stringExpectedValue, typeof(TProperty));
        }
        catch (FormatException ex)
        {
            throw new CannotConvertColumnValueToPropertyTypeException(
                stringExpectedValue,
                _memberName,
                typeof(TProperty),
                typeof(T),
                ex
            );
        }
    }

    public bool IsMappedTo(string columnName) => ColumnOrMemberName.EqualsHumanized(columnName);

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

    public virtual bool Equals(PropertyDefinition<T, TProperty>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Expression.ToString().Equals(other.Expression.ToString())
            && ColumnOrMemberName.EqualsHumanized(other.ColumnOrMemberName);
    }

    public override int GetHashCode() => HashCode.Combine(Expression.ToString(), ColumnOrMemberName);

    public override string ToString() => $"{typeof(T).Name}.{_memberName} -> [{ColumnOrMemberName}]";
}