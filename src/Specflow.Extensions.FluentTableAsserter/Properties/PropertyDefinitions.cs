using System.Collections.Generic;
using System.Linq;
using Specflow.Extensions.FluentTableAsserter.Exceptions;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

internal class PropertyDefinitions<T>
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

    public void EnsureColumnAreCorrectlyMapped(IEnumerable<string> headers)
    {
        var notMappedHeaders = headers
            .Where(header => !_ignoredColumns.Contains(header))
            .Where(header => !_propertyDefinitions.Any(p => p.IsMappedTo(header)))
            .ToArray();

        if (notMappedHeaders.Any())
        {
            throw new MissingColumnDefinitionException(typeof(T), notMappedHeaders.First());
        }
    }

    public IEnumerable<IPropertyDefinition<T>> ForColumn(string headerName) =>
        _propertyDefinitions.Where(x => x.IsMappedTo(headerName));
}