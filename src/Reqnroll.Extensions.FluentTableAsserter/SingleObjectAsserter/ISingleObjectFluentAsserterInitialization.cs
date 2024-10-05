using System;
using System.Linq.Expressions;

namespace Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter;

public interface ISingleObjectFluentAsserterInitialization<T>
{
    ISingleObjectFluentAsserter<T> WithProperty<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression
    ) =>
        WithProperty(propertyExpression, x => x);

    ISingleObjectFluentAsserter<T> WithProperty<TProperty, TTransformedProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        SingleObjectConfiguration<T, TProperty, TTransformedProperty>? configure = default
    );
}

public delegate ISingleObjectPropertyConfiguration<T, TTransformedProperty> SingleObjectConfiguration<T, TProperty,
    TTransformedProperty>(
    ISingleObjectPropertyConfiguration<T, TProperty> value
);