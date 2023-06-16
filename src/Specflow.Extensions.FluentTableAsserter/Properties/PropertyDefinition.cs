using System;
using System.Linq.Expressions;
using Specflow.Extensions.FluentTableAsserter.Asserters;
using Specflow.Extensions.FluentTableAsserter.Exceptions;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyDefinition<T, TProperty>(
    Expression<Func<T, TProperty>> Expression,
    PropertyConfiguration<T, TProperty> Configuration
) : IPropertyDefinition<T>
{
    private readonly string _propertyName = FindPropertyName(Expression);
    private readonly Func<T, TProperty> _getPropertyValue = Expression.Compile();

    private string ColumnOrMemberName => Configuration.ColumnName ?? _propertyName;

    public AssertionResult AssertEquivalent(string stringExpectedValue, T data)
    {
        var expectedValue = ConvertToPropertyType(stringExpectedValue);
        var actualValue = _getPropertyValue(data);

        if (expectedValue is null && actualValue is null)
        {
            return AssertionResult.Success;
        }

        if (actualValue!.Equals(expectedValue))
        {
            return AssertionResult.Success;
        }

        return AssertionResult.Fail(_propertyName, actualValue);
    }

    private TProperty ConvertToPropertyType(string stringExpectedValue)
    {
        try
        {
            if (Configuration.ColumnValueConvertion is not null)
            {
                return Configuration.ColumnValueConvertion(stringExpectedValue);
            }

            return (TProperty)Convert.ChangeType(stringExpectedValue, typeof(TProperty));
        }
        catch (FormatException ex)
        {
            throw new CannotConvertColumnValueToPropertyTypeException(
                stringExpectedValue,
                _propertyName,
                typeof(TProperty),
                typeof(T),
                ex
            );
        }
    }

    public bool IsMappedTo(string columnName) => ColumnOrMemberName.EqualsHumanized(columnName);

    private static string FindPropertyName(Expression expression)
    {
        string? result;
        try
        {
            result = expression switch
            {
                MemberExpression e => e.Member.Name,
                MethodCallExpression e => e.Method.Name,
                LambdaExpression e => FindPropertyName(e.Body),
                UnaryExpression e => FindPropertyName(e.Operand),
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

    public override string ToString() => $"{typeof(T).Name}.{_propertyName} -> [{ColumnOrMemberName}]";
}