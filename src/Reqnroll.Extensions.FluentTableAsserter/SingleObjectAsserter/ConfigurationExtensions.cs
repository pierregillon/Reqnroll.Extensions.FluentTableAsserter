using System;
using System.Collections.Generic;
using System.Linq;
using Reqnroll.Extensions.FluentTableAsserter.CollectionAsserters;

namespace Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter;

public static class ConfigurationExtensions
{
    /// <summary>
    ///     Split the field value with the provided separator.
    ///     IE: allows to automatically convert value "john, sam, eric" into enumerable ["john", "sam", "eric"].
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="splitCharacter"></param>
    /// <typeparam name="TObject"></typeparam>
    /// <returns></returns>
    public static ISingleObjectPropertyConfiguration<TObject, IEnumerable<string>> SplitFieldValueBySeparator<TObject>(
        this ISingleObjectPropertyConfiguration<TObject, IEnumerable<string>> configuration,
        string splitCharacter = ","
    ) =>
        configuration
            .WithFieldToPropertyConversion(
                fieldValue => fieldValue
                    .Split(splitCharacter, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
            );

    /// <summary>
    ///     Split the cell value with the provided separator.
    ///     IE: allows to automatically convert value "john, sam, eric" into enumerable ["john", "sam", "eric"].
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="splitCharacter"></param>
    /// <typeparam name="TCollection"></typeparam>
    /// <returns></returns>
    public static ICollectionPropertyConfiguration<TCollection, IEnumerable<string>>
        SplitCellValueBySeparator<TCollection>(
            this ICollectionPropertyConfiguration<TCollection, IEnumerable<string>> configuration,
            string splitCharacter = ","
        ) =>
        configuration
            .WithCellToPropertyConversion(
                fieldValue => fieldValue
                    .Split(splitCharacter, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
            );

    /// <summary>
    ///     Split the cell value with the provided separator.
    ///     IE: allows to automatically convert value "john, sam, eric" into enumerable ["john", "sam", "eric"].
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="splitCharacter"></param>
    /// <typeparam name="TCollection"></typeparam>
    /// <typeparam name="TEnum">The enum value</typeparam>
    /// <returns></returns>
    public static ICollectionPropertyConfiguration<TCollection, IEnumerable<TEnum>> SplitCellValueBySeparator<
        TCollection, TEnum>(
        this ICollectionPropertyConfiguration<TCollection, IEnumerable<TEnum>> configuration,
        string splitCharacter = ","
    ) where TEnum : Enum =>
        configuration
            .WithCellToPropertyConversion(
                fieldValue => fieldValue
                    .Split(splitCharacter, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Select(HumanReadableExtensions.ParseEnum<TEnum>)
            );
}