using System;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

public interface ISingleObjectPropertyConfiguration<T, in TProperty>
{
    public ISingleObjectPropertyConfiguration<T, TProperty> ComparedToField(string columnName);

    public ISingleObjectPropertyConfiguration<T, TProperty> WithFieldValueConversion(
        Func<string, TProperty> conversion
    );
}