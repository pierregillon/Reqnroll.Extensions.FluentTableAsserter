using System;
using System.Linq.Expressions;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

public interface ISingleObjectFluentAsserterInitialization<T>
{
    ISingleObjectFluentAsserter<T> WithProperty<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression
    ) =>
        WithProperty(propertyExpression, x => x);

    ISingleObjectFluentAsserter<T> WithProperty<TProperty, TConvertedProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        SingleObjectConfiguration<T, TProperty, TConvertedProperty>? configure = default
    );
}

public delegate ISingleObjectPropertyConfiguration<T, TConvertedProperty> SingleObjectConfiguration<T, TProperty,
    TConvertedProperty>(
    ISingleObjectPropertyConfiguration<T, TProperty> value
);