using System;
using System.Linq;
using System.Linq.Expressions;
using Specflow.Extensions.FluentTableAsserter.Properties;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

public class SingleObjectAsserter<TElement> : ISingleObjectFluentAsserter<TElement>
{
    private readonly Table _table;
    private readonly TElement _actualElement;
    private readonly PropertyDefinitions<TElement> _propertyDefinitions = new();

    public SingleObjectAsserter(Table table, TElement actualElement)
    {
        _table = table;
        _actualElement = actualElement;
    }

    public ISingleObjectFluentAsserter<TElement> WithProperty<TProperty>(
        Expression<Func<TElement, TProperty>> propertyExpression,
        Func<ISingleObjectPropertyConfiguration<TElement, TProperty>,
            ISingleObjectPropertyConfiguration<TElement, TProperty>>? configure = null
    )
    {
        var configuration = configure is not null
            ? configure(PropertyConfiguration<TElement, TProperty>.Default)
            : PropertyConfiguration<TElement, TProperty>.Default;

        var cast = (PropertyConfiguration<TElement, TProperty>)configuration;

        _propertyDefinitions.Add(new PropertyDefinition<TElement, TProperty>(propertyExpression, cast));

        return this;
    }

    public ISingleObjectFluentAsserter<TElement> IgnoringField(string columnName)
    {
        _propertyDefinitions.AddIgnoredColumnName(columnName);
        return this;
    }

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
                    throw new ExpectedTableNotEquivalentToObjectException(
                        result.MemberName,
                        result.StringActualValue,
                        fieldName,
                        result.StringExpectedValue
                    );
                }
            }
        }
    }
}