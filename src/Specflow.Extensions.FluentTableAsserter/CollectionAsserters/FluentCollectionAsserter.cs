using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;
using Specflow.Extensions.FluentTableAsserter.Properties;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public class FluentCollectionAsserter<T> : IFluentAsserter<T>
{
    private readonly Table _table;
    private readonly IEnumerable<T> _actualValues;
    private readonly CollectionPropertyDefinitions<T> _collectionPropertyDefinitions = new();

    internal FluentCollectionAsserter(Table table, IEnumerable<T> actualValues)
    {
        _table = table;
        _actualValues = actualValues;
    }

    public IFluentAsserter<T> WithProperty<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        Func<PropertyConfiguration<T, TProperty>, PropertyConfiguration<T, TProperty>>? configure = null
    )
    {
        var configuration = configure is not null
            ? configure(PropertyConfiguration<T, TProperty>.Default)
            : PropertyConfiguration<T, TProperty>.Default;

        _collectionPropertyDefinitions.Add(new PropertyDefinition<T, TProperty>(propertyExpression, configuration));

        return this;
    }

    public IFluentAsserter<T> IgnoringColumn(string columnName)
    {
        _collectionPropertyDefinitions.AddIgnoredColumnName(columnName);
        return this;
    }

    public void AssertEquivalent() => Assert();

    public void Assert()
    {
        _collectionPropertyDefinitions.EnsureColumnAreCorrectlyMapped(_table.Header);

        if (_table.RowCount != _actualValues.Count())
        {
            throw new TableRowCountIsDifferentThanElementCountException<T>(_table.RowCount, _actualValues.Count());
        }

        for (var rowIndex = 0; rowIndex < _table.Rows.Count; rowIndex++)
        {
            var row = _table.Rows[rowIndex];

            var data = _actualValues.ElementAt(rowIndex);

            foreach (var columnName in _table.Header)
            {
                var propertyDefinitions = _collectionPropertyDefinitions.ForColumn(columnName);

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
                            expectedValue
                        );
                    }
                }
            }
        }
    }
}