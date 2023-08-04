using System;
using System.Linq.Expressions;
using Specflow.Extensions.FluentTableAsserter.Properties;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public interface IFluentAsserterInitialization<T>
{
    IFluentAsserter<T> WithProperty<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        Func<PropertyConfiguration<T, TProperty>, PropertyConfiguration<T, TProperty>>? configure = null
    );
}