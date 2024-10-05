using System;
using System.Linq;
using System.Linq.Expressions;
using Reqnroll.Extensions.FluentTableAsserter.Properties;
using Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

namespace Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter;

public class SingleObjectAsserter<TElement>(Table table, TElement actualElement) : ISingleObjectFluentAsserter<TElement>
{
    private readonly PropertyDefinitions<TElement> _propertyDefinitions = new();

    public ISingleObjectFluentAsserter<TElement> WithProperty<TProperty, TTransformedProperty>(
        Expression<Func<TElement, TProperty>> propertyExpression,
        SingleObjectConfiguration<TElement, TProperty, TTransformedProperty>? configure = default
    )
    {
        var configuration = configure is not null
            ? configure(PropertyConfigurationBuilder<TElement, TProperty>.Default)
            : PropertyConfigurationBuilder<TElement, TTransformedProperty>.Default;

        var cast = (PropertyConfigurationBuilder<TElement, TTransformedProperty>)configuration;

        _propertyDefinitions.Add(new PropertyDefinition<TElement, TProperty>(propertyExpression, cast.Value));

        return this;
    }

    public ISingleObjectFluentAsserter<TElement> IgnoringField(string columnName)
    {
        _propertyDefinitions.AddIgnoredColumnName(columnName);
        return this;
    }

    public void Assert()
    {
        if (table.Header.Count != 2)
        {
            throw new InvalidColumnCountForObjectAssertionException();
        }

        if (!table.Rows.Any())
        {
            throw new NoFieldToEvaluateException();
        }

        var fieldNames = table.Rows
            .Select(x => x.Values.First())
            .Distinct()
            .ToArray();

        _propertyDefinitions.EnsureColumnAreCorrectlyMapped(fieldNames);

        foreach (var rowGroup in table.Rows.GroupBy(x => x.Values.First(), x => x.Values.Last()))
        {
            var fieldName = rowGroup.Key;
            var expectedValue = string.Join(Environment.NewLine, rowGroup.Select(x => x).ToArray());
            var propertyDefinitions = _propertyDefinitions.ForColumn(fieldName);

            foreach (var propertyDefinition in propertyDefinitions)
            {
                var result = propertyDefinition.AssertEquivalent(expectedValue, actualElement);

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