using System;
using System.Linq.Expressions;

namespace Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

internal class MemberNameNotFoundOnExpressionException: Exception
{
    public MemberNameNotFoundOnExpressionException(Expression expression)
        :base(BuildMessage(expression))
    {
    }

    public MemberNameNotFoundOnExpressionException(Expression expression, Exception innerException)
        :base(BuildMessage(expression), innerException)
    {
    }

    private static string BuildMessage(Expression expression) =>
        $"[SpecFlow] unable to find a member name for expression {expression}";
}