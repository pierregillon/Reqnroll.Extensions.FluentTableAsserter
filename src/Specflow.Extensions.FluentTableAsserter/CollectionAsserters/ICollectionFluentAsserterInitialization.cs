using System;
using System.Linq.Expressions;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public interface ICollectionFluentAsserterInitialization<T>
{
    ICollectionCollectionFluentAsserter<T> WithProperty<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        Func<ICollectionPropertyConfiguration<T, TProperty>, ICollectionPropertyConfiguration<T, TProperty>>?
            configure = null
    );
}