using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyDefinition<T, TProperty>(
    Expression<Func<T, TProperty>> Expression,
    PropertyConfiguration Configuration
) : IPropertyDefinition<T>
{
    public string PropertyName { get; } = FindPropertyName(Expression);
    private readonly Func<T, TProperty> _getPropertyValue = Expression.Compile();

    private string ColumnName => Configuration.ColumnName ?? PropertyName;

    public AssertionResult AssertEquivalent(string stringExpectedValue, T data)
    {
        var actualValue = ConvertPropertyValue(_getPropertyValue(data));

        var expectedValue = ConvertColumnValue(
            stringExpectedValue,
            actualValue?.GetType() ?? typeof(TProperty)
        );

        return AreEquivalent(actualValue, expectedValue)
            ? AssertionResult.Success
            : AssertionResult.Fail(Expression.Body, actualValue, expectedValue);
    }

    private static bool AreEquivalent(object? actualValue, object? expectedValue)
    {
        if (expectedValue is null && actualValue is null)
        {
            return true;
        }

        var actualType = actualValue?.GetType() ?? typeof(TProperty);

        if (actualType == typeof(string))
        {
            var actual = (string?)actualValue;
            var expected = (string?)expectedValue;

            if (string.IsNullOrEmpty(actual) && string.IsNullOrEmpty(expected))
            {
                return true;
            }
        }

        if (expectedValue is null || actualValue is null)
        {
            return false;
        }

        if (actualType.IsEnumerableType())
        {
            var actual = (IEnumerable)actualValue;
            var expected = (IEnumerable)expectedValue;

            return Equivalent(actual, expected);
        }

        return actualValue.Equals(expectedValue);
    }

    private static bool Equivalent(IEnumerable actual, IEnumerable expected)
    {
        var actualArray = actual.Enumerate().ToArray();
        var expectedArray = expected.Enumerate().ToArray();

        if (actualArray.Length != expectedArray.Length)
        {
            return false;
        }

        if (actualArray.Zip(expectedArray, (x, y) => (x, y)).Any(value => !Equals(value.x, value.y)))
        {
            return false;
        }

        return true;
    }

    private object? ConvertPropertyValue(TProperty property)
    {
        if (Configuration.PropertyConversion is not null)
        {
            return Configuration.PropertyConversion?.Invoke(property);
        }

        return property;
    }

    private object? ConvertColumnValue(string stringExpectedValue, Type propertyType)
    {
        try
        {
            if (Configuration.ColumnToPropertyConversion is not null)
            {
                return Configuration.ColumnToPropertyConversion(stringExpectedValue);
            }

            if (propertyType.IsEnum)
            {
                return HumanReadableExtensions.ParseEnum(propertyType, stringExpectedValue);
            }

            return Convert.ChangeType(stringExpectedValue, propertyType);
        }
        catch (FormatException ex)
        {
            throw new CannotConvertCellValueToPropertyTypeException(
                stringExpectedValue,
                PropertyName,
                propertyType,
                typeof(T),
                ex
            );
        }
        catch (InvalidCastException ex)
        {
            throw new CannotConvertCellValueToPropertyTypeException(
                stringExpectedValue,
                PropertyName,
                propertyType,
                typeof(T),
                ex
            );
        }
    }

    public bool IsMappedTo(string columnName) => ColumnName.EqualsHumanReadable(columnName);

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
            throw new MemberNameNotFoundOnExpressionException(expression, e);
        }

        if (result == null)
        {
            throw new MemberNameNotFoundOnExpressionException(expression);
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
            && ColumnName.EqualsHumanReadable(other.ColumnName);
    }

    public override int GetHashCode() => HashCode.Combine(Expression.ToString(), ColumnName);

    public override string ToString() => $"{typeof(T).Name}.{PropertyName} -> [{ColumnName}]";
}