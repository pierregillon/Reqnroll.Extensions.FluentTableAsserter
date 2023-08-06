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
        var validHeaders = fieldNames
            .Where(header => !_ignoredColumns.Contains(header))
            .ToArray();

        EnsureNoMissingColumns(validHeaders);
        EnsureNoDuplicates(validHeaders);
    }

    public IEnumerable<IPropertyDefinition<T>> ForColumn(string headerName) =>
        _propertyDefinitions.Where(x => x.IsMappedTo(headerName));

    private void EnsureNoMissingColumns(IEnumerable<string> validHeaders)
    {
        var notMappedHeaders = validHeaders
            .Where(header => !_propertyDefinitions.Any(p => p.IsMappedTo(header)))
            .ToArray();

        if (notMappedHeaders.Any())
        {
            throw new MissingColumnDefinitionException(typeof(T), notMappedHeaders.First());
        }
    }

    private void EnsureNoDuplicates(IEnumerable<string> validHeaders)
    {
        var headersGroupedByNormalizedForm = validHeaders
            .GroupBy(x => x.FromHumanReadableToNormalized())
            .Where(x => x.Count() >= 2)
            .Select(x => new
            {
                _propertyDefinitions.First().PropertyName,
                DuplicatedColumns = x.ToArray()
            })
            .ToArray();

        if (headersGroupedByNormalizedForm.Any())
        {
            var first = headersGroupedByNormalizedForm.First();
            throw new DuplicateColumnDefinitionException(typeof(T), first.DuplicatedColumns, first.PropertyName);
        }
    }
}