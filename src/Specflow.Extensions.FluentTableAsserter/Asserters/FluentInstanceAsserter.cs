using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Specflow.Extensions.FluentTableAsserter.Exceptions;
using Specflow.Extensions.FluentTableAsserter.Properties;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter.Asserters;

public class FluentInstanceAsserter<TElement> : IFluentAsserter<TElement>
{
    private readonly Table _table;
    private readonly TElement _actualElement;
    private readonly InstancePropertyDefinitions<TElement> _propertyDefinitions = new();

    public FluentInstanceAsserter(Table table, TElement actualElement)
    {
        _table = table;
        _actualElement = actualElement;
    }

    public IFluentAsserter<TElement> WithProperty<TProperty>(
        Expression<Func<TElement, TProperty>> propertyExpression,
        Func<PropertyConfiguration<TElement, TProperty>, PropertyConfiguration<TElement, TProperty>>? configure = null
    )
    {
        var configuration = configure is not null
            ? configure(PropertyConfiguration<TElement, TProperty>.Default)
            : PropertyConfiguration<TElement, TProperty>.Default;

        _propertyDefinitions.Add(new PropertyDefinition<TElement, TProperty>(propertyExpression, configuration));

        return this;
    }

    public IFluentAsserter<TElement> IgnoringColumn(string columnName)
    {
        _propertyDefinitions.AddIgnoredColumnName(columnName);
        return this;
    }

    public void AssertEquivalent() => Assert();

    public void Assert()
    {
        if (_table.Header.Count != 2)
        {
            throw new InvalidColumnCountForObjectAssertionException();
        }

        if (!_table.Rows.Any())
        {
            throw new NoFieldToEvaluateException();
        }

        var fieldNames = _table.Rows
            .Select(x => x.Values.First())
            .Distinct()
            .ToArray();

        _propertyDefinitions.EnsureColumnAreCorrectlyMapped(fieldNames);

        foreach (var rowGroup in _table.Rows.GroupBy(x => x.Values.First(), x => x.Values.Last()))
        {
            var fieldName = rowGroup.Key;
            var expectedValue = string.Join(Environment.NewLine, rowGroup.Select(x => x).ToArray());
            var propertyDefinitions = _propertyDefinitions.ForColumn(fieldName);

            foreach (var propertyDefinition in propertyDefinitions)
            {
                var result = propertyDefinition.AssertEquivalent(expectedValue, _actualElement);

                if (!result.IsSuccess)
                {
                    throw new ExpectedTableNotEquivalentToInstanceException(
                        result.MemberName,
                        result.StringActualValue,
                        fieldName,
                        expectedValue
                    );
                }
            }
        }
    }
}

public class NoFieldToEvaluateException : Exception
{
    private const string GenericMessage = "You must define at least one field to assert the object to.";

    public NoFieldToEvaluateException() : base(GenericMessage)
    {

    }
}

public class InvalidColumnCountForObjectAssertionException : Exception
{
    private const string GenericMessage =
        "You must define strictly 2 columns: first of the field name (ie: Field), second for field value (ie: Value). Column names are flexible.";

    public InvalidColumnCountForObjectAssertionException() : base(GenericMessage)
    {

    }
}

public class ExpectedTableNotEquivalentToInstanceException : Exception
{
    public ExpectedTableNotEquivalentToInstanceException(
        string memberName,
        object? actualValue,
        string columnName,
        string expectedValue
    )
        : base(
            $"'{memberName}' actual data is '{actualValue}' but should be '{expectedValue}' from column '{columnName}'.")
    {
    }
}

internal class InstancePropertyDefinitions<T>
{
    private readonly List<string> _ignoredColumns = new();
    private readonly List<IPropertyDefinition<T>> _propertyDefinitions = new();

    public void Add<TProperty>(PropertyDefinition<T, TProperty> propertyDefinition)
    {
        if (_propertyDefinitions.Any(x => x.Equals(propertyDefinition)))
        {
            throw new PropertyDefinitionAlreadyExistsException(propertyDefinition.ToString());
        }

        _propertyDefinitions.Add(propertyDefinition);
    }

    public void AddIgnoredColumnName(string columnName) => _ignoredColumns.Add(columnName);

    public void EnsureColumnAreCorrectlyMapped(IEnumerable<string> fieldNames)
    {
        var notMappedHeaders = fieldNames
            .Where(fieldName => !_ignoredColumns.Contains(fieldName))
            .Where(fieldName => !_propertyDefinitions.Any(p => p.IsMappedTo(fieldName)))
            .ToArray();

        if (notMappedHeaders.Any())
        {
            throw new MissingColumnDefinitionException(typeof(T), notMappedHeaders.First());
        }
    }

    public IEnumerable<IPropertyDefinition<T>> ForColumn(string headerName) =>
        _propertyDefinitions.Where(x => x.IsMappedTo(headerName));
}