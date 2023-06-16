using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter;

public static class TableExtensions
{
    public static IFluentAsserterInitialization<TElement> ShouldMatch<TElement>(
        this Table table,
        IEnumerable<TElement> actualValues
    )
        => new FluentAsserter<TElement>(table, actualValues);
}

public class FluentAsserter<T> : IFluentAsserter<T>, IFluentAsserterInitialization<T>
{
    private readonly Table _table;
    private readonly IEnumerable<T> _actualValues;
    private readonly List<IPropertyDefinition<T>> _propertyDefinitions = new();

    public FluentAsserter(Table table, IEnumerable<T> actualValues)
    {
        _table = table;
        _actualValues = actualValues;
    }

    public IFluentAsserter<T> WithProperty<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        _propertyDefinitions.Add(new PropertyDefinition<T, TProperty>(typeof(TProperty), propertyExpression));
        return this;
    }

    public void AssertEquivalent()
    {
        for (var rowIndex = 0; rowIndex < _table.Rows.Count; rowIndex++)
        {
            var row = _table.Rows[rowIndex];
            var data = _actualValues.ElementAt(rowIndex);
            for (var headerIndex = 0; headerIndex < _table.Rows.Count; headerIndex++)
            {
                var propertyDefinition = _propertyDefinitions[headerIndex];
                var expectedValue = row[headerIndex];

                var result = propertyDefinition.AssertEquivalent(expectedValue, data);

                if (!result.IsSuccess)
                {
                    throw new ExpectedTableNotEquivalentToDataException(
                        rowIndex,
                        result.MemberName,
                        result.ActualValue,
                        _table.Header.ElementAt(headerIndex),
                        expectedValue
                    );
                }
            }
        }
    }
}

public interface IFluentAsserter<T>
{
    IFluentAsserter<T> WithProperty<TProperty>(Expression<Func<T, TProperty>> propertyExpression);
    void AssertEquivalent();
}

public interface IFluentAsserterInitialization<T>
{
    IFluentAsserter<T> WithProperty<TProperty>(Expression<Func<T, TProperty>> propertyExpression);
}

public record PropertyDefinition<T, TProperty> : IPropertyDefinition<T>
{
    private readonly Type _type;
    private readonly Expression _expression;
    private readonly string _memberName;
    private readonly Func<T, TProperty> _func;

    public PropertyDefinition(Type type, Expression<Func<T, TProperty>> expression)
    {
        _type = type;
        _expression = expression;
        _memberName = FindMemberName(expression);
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

    private static string FindMemberName(Expression expression)
    {
        string? result;
        try
        {
            result = expression switch
            {
                MemberExpression e => e.Member.Name,
                MethodCallExpression e => e.Method.Name,
                LambdaExpression e => FindMemberName(e.Body),
                UnaryExpression e => FindMemberName(e.Operand),
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

public record AssertionResult(bool IsSuccess, string MemberName, object? ActualValue)
{
    public static AssertionResult Success
        => new(true, string.Empty, null);

    public static AssertionResult Fail(string memberName, object? actualValue)
        => new(false, memberName, actualValue);
}

public interface IPropertyDefinition<T>
{
    AssertionResult AssertEquivalent(string expectedValue, T element);
}

public class ExpectedTableNotEquivalentToDataException : Exception
{
    public ExpectedTableNotEquivalentToDataException(
        int index,
        string memberName,
        object? actualValue,
        string headerName,
        string expectedValue
    )
        : base(
            $"At index {index}, '{memberName}' actual data is '{actualValue}' but should be '{expectedValue}' from column '{headerName}'.")
    {
    }
}