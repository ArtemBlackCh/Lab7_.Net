namespace CityAlerts;

public interface IAlertSubscriber
{
    string Name { get; }
    void OnAlert(object sender, WeatherEventArgs e);
}

public interface ILoggable
{
    IReadOnlyList<string> Log { get; }
    void WriteLog(string message);
}

public interface IActionExecutor
{
    string ActionDescription { get; }
    void Execute();
}
