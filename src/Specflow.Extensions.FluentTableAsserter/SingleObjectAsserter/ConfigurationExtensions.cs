using System.Collections.Generic;
using System.Linq;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

public static class ConfigurationExtensions
{
    /// <summary>
    ///     Split the field value with the provided separator.
    ///     IE: allows to automatically convert value "john, sam, eric" into enumerable ["john", "sam", "eric"].
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="splitCharacter"></param>
    /// <typeparam name="TField"></typeparam>
    /// <returns></returns>
    public static ISingleObjectPropertyConfiguration<TField, IEnumerable<string>> SplitFieldValueBySeparator<TField>(
        this ISingleObjectPropertyConfiguration<TField, IEnumerable<string>> configuration,
        string splitCharacter = ","
    ) =>
        configuration
            .WithFieldToPropertyConversion(
                fieldValue => fieldValue
                    .Split(splitCharacter)
                    .Select(x => x.Trim())
            );

    /// <summary>
    ///     Split the cell value with the provided separator.
    ///     IE: allows to automatically convert value "john, sam, eric" into enumerable ["john", "sam", "eric"].
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="splitCharacter"></param>
    /// <typeparam name="TField"></typeparam>
    /// <returns></returns>
    public static ICollectionPropertyConfiguration<TField, IEnumerable<string>> SplitCellValueBySeparator<TField>(
        this ICollectionPropertyConfiguration<TField, IEnumerable<string>> configuration,
        string splitCharacter = ","
    ) =>
        configuration
            .WithColumnToPropertyConversion(
                fieldValue => fieldValue
                    .Split(splitCharacter)
                    .Select(x => x.Trim())
            );
}