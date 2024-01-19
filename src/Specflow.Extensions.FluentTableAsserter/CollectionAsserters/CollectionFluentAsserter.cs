using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;
using Specflow.Extensions.FluentTableAsserter.Properties;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public class CollectionFluentAsserter<T>(Table table, IEnumerable<T> actualValues)
    : ICollectionCollectionFluentAsserter<T>
{
    private readonly PropertyDefinitions<T> _propertyDefinitions = new();

    public ICollectionCollectionFluentAsserter<T> WithProperty<TProperty, TTransformedProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        CollectionConfiguration<T, TProperty, TTransformedProperty>? configure = null
    )
    {
        var configuration = configure is not null
            ? configure(PropertyConfigurationBuilder<T, TProperty>.Default)
            : PropertyConfigurationBuilder<T, TTransformedProperty>.Default;

        var cast = (PropertyConfigurationBuilder<T, TTransformedProperty>)configuration;

        _propertyDefinitions.Add(new PropertyDefinition<T, TProperty>(propertyExpression, cast.Value));

        return this;
    }

    public ICollectionCollectionFluentAsserter<T> IgnoringColumn(string columnName)
    {
        _propertyDefinitions.AddIgnoredColumnName(columnName);
        return this;
    }

    public void Assert()
    {
        _propertyDefinitions.EnsureColumnAreCorrectlyMapped(table.Header);

        if (table.RowCount != actualValues.Count())
        {
            throw new TableRowCountIsDifferentThanElementCountException(table.RowCount, typeof(T),
                actualValues.Count());
        }

        for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
        {
            var row = table.Rows[rowIndex];

            var data = actualValues.ElementAt(rowIndex);

            foreach (var columnName in table.Header)
            {
                var propertyDefinitions = _propertyDefinitions.ForColumn(columnName);

                foreach (var propertyDefinition in propertyDefinitions)
                {
                    var expectedValue = row[columnName];

                    var result = propertyDefinition.AssertEquivalent(expectedValue, data);

                    if (!result.IsSuccess)
                    {
                        throw new ExpectedTableNotEquivalentToCollectionItemException(
                            rowIndex,
                            result.MemberName,
                            result.StringActualValue,
                            columnName,
                            result.StringExpectedValue
                        );
                    }
                }
            }
        }
    }
}