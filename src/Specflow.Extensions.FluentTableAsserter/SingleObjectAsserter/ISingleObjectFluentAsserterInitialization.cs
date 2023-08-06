using System;
using System.Linq.Expressions;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

public interface ISingleObjectFluentAsserterInitialization<T>
{
    ISingleObjectFluentAsserter<T> WithProperty<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        Func<ISingleObjectPropertyConfiguration<T, TProperty>, ISingleObjectPropertyConfiguration<T, TProperty>>?
            configure = null
    );
}