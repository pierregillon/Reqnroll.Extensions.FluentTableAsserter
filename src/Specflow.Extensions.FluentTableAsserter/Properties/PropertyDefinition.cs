using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyDefinition<T, TProperty>(
    Expression<Func<T, TProperty>> Expression,
    PropertyConfiguration<T, TProperty> Configuration
) : IPropertyDefinition<T>
{
    public string PropertyName { get; } = FindPropertyName(Expression);
    private readonly Func<T, TProperty> _getPropertyValue = Expression.Compile();

    private string ColumnName => Configuration.ColumnName ?? PropertyName;

    public AssertionResult AssertEquivalent(string stringExpectedValue, T data)
    {
        var expectedValue = ConvertToPropertyType(stringExpectedValue);
        var actualValue = _getPropertyValue(data);

        if (expectedValue is null && actualValue is null)
        {
            return AssertionResult.Success;
        }

        if (typeof(TProperty) == typeof(string))
        {
            var actual = (string?)(object?)actualValue;
            var expected = (string?)(object?)expectedValue;

            if (string.IsNullOrEmpty(actual) && string.IsNullOrEmpty(expected))
            {
                return AssertionResult.Success;
            }
        }

        if (expectedValue is null || actualValue is null)
        {
            return AssertionResult.Fail(PropertyName, actualValue);
        }

        if (typeof(TProperty).IsEnumerableType())
        {
            var actual = (IEnumerable)actualValue;
            var expected = (IEnumerable)expectedValue;

            var actualArray = actual.Enumerate().ToArray();
            var expectedArray = expected.Enumerate().ToArray();

            if (actualArray.Length != expectedArray.Length)
            {
                return AssertionResult.Fail(PropertyName, actualValue);
            }

            foreach (var value in actualArray.Zip(expectedArray, (x, y) => (x, y)))
            {
                if (!Equals(value.x, value.y))
                {
                    return AssertionResult.Fail(PropertyName, actualValue);
                }
            }

            return AssertionResult.Success;
        }

        if (actualValue.Equals(expectedValue))
        {
            return AssertionResult.Success;
        }

        return AssertionResult.Fail(PropertyName, actualValue);
    }

    private TProperty ConvertToPropertyType(string stringExpectedValue)
    {
        try
        {
            if (Configuration.ColumnValueConversion is not null)
            {
                return Configuration.ColumnValueConversion(stringExpectedValue);
            }

            if (typeof(TProperty).IsEnum)
            {
                if (!HumanReadableExtensions.TryParseEnum(typeof(TProperty), stringExpectedValue, out var enumValue))
                {
                    throw new CannotParseEnumToEnumValuException<TProperty>(stringExpectedValue);
                }

                return (TProperty)enumValue;
            }

            return (TProperty)Convert.ChangeType(stringExpectedValue, typeof(TProperty));
        }
        catch (FormatException ex)
        {
            throw new CannotConvertCellValueToPropertyTypeException(
                stringExpectedValue,
                PropertyName,
                typeof(TProperty),
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
            && ColumnName.EqualsHumanReadable(other.ColumnName);
    }

    public override int GetHashCode() => HashCode.Combine(Expression.ToString(), ColumnName);

    public override string ToString() => $"{typeof(T).Name}.{PropertyName} -> [{ColumnName}]";
}