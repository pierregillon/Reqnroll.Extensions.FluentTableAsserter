using System;
using System.Linq.Expressions;

namespace Specflow.Extensions.FluentTableAsserter;

public interface IFluentAsserterInitialization<T>
{
    IFluentAsserter<T> WithProperty<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        Func<PropertyConfiguration<T, TProperty>, PropertyConfiguration<T, TProperty>>? configure = null
    );
}