using System.Collections.Generic;
using System.Linq;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;
using Specflow.Extensions.FluentTableAsserter.Properties;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

internal class SingleObjectPropertyDefinitions<T>
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