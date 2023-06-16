using FluentAssertions;
using FluentAssertions.Primitives;

namespace Specflow.Extensions.FluentTableAsserter.Tests;

public static class FluentAssertionExtensions
{
    public static CodeAssertions NotCompile(this StringAssertions assertions)
    {
        var code = assertions.Subject;

        var errors = SourceCodeCompiler.Compile(code).ToArray();

        errors
            .Should()
            .NotBeEmpty();

        return new CodeAssertions(code, errors);
    }

    public record CodeAssertions(string _, IReadOnlyCollection<string> Errors)
    {
        public void WithErrors(params string[] expectedErrors) => Errors
            .Should()
            .BeEquivalentTo(expectedErrors);
    }
}