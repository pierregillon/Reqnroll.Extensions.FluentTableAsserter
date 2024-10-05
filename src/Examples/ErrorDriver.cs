using Reqnroll;

namespace Examples;

public class ErrorDriver
{
    private readonly ScenarioInfo _scenarioInfo;

    public ErrorDriver(ScenarioInfo scenarioInfo) => _scenarioInfo = scenarioInfo;

    public Exception? LastError { get; private set; }

    public void TryExecute(Action action)
    {
        if (!_scenarioInfo.Tags.Contains("ErrorHandling"))
        {
            action();
            return;
        }

        try
        {
            action();
            throw new InvalidOperationException("Reqnroll: error should have occurred");
        }
        catch (Exception e)
        {
            LastError = e;
        }
    }
}