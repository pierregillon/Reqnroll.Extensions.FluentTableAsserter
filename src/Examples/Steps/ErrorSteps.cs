using FluentAssertions;
using Reqnroll;

namespace Examples.Steps;

[Binding]
public class ErrorSteps
{
    private readonly ErrorDriver _errorDriver;

    public ErrorSteps(ErrorDriver errorDriver) => _errorDriver = errorDriver;

    [Then(@"an error occurred with ""(.*)""")]
    public void ThenAnErrorOccurredWith(string errorMessage)
    {
        _errorDriver.LastError.Should().NotBeNull($"An error was expected to occurred with message '{errorMessage}'.");
        _errorDriver.LastError!.Message.Should().Be(errorMessage);
    }
}