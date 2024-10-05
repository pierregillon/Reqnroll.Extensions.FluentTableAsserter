using System;
using System.Linq.Expressions;

namespace Reqnroll.Extensions.FluentTableAsserter.CollectionAsserters;

public interface ICollectionFluentAsserterInitialization<T>
{
    ICollectionCollectionFluentAsserter<T> WithProperty<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression
    ) =>
        WithProperty(propertyExpression, x => x);

    ICollectionCollectionFluentAsserter<T> WithProperty<TProperty, TTransformedProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        CollectionConfiguration<T, TProperty, TTransformedProperty>? configure = default
    );
}

public delegate ICollectionPropertyConfiguration<T, TTransformedProperty> CollectionConfiguration<T, TProperty,
    TTransformedProperty>(
    ICollectionPropertyConfiguration<T, TProperty> value
);