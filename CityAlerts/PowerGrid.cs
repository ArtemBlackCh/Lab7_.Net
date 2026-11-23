namespace CityAlerts;

public class PowerGrid : IAlertSubscriber, ILoggable, IActionExecutor
{
    private readonly List<string> _log = new();

    public event Action<string>? MessageGenerated;

    public string Name => "Електромережі";
    public bool EnhancedMode { get; private set; }
    public IReadOnlyList<string> Log => _log;
    public string ActionDescription => "Перейти в посилений режим";

    public void OnAlert(object sender, WeatherEventArgs e)
    {
        SwitchToEnhancedMode();
    }

    public void SwitchToEnhancedMode()
    {
        EnhancedMode = true;
        WriteLog($"{Name}: {ActionDescription}!");
    }

    public void Execute()
    {
        SwitchToEnhancedMode();
    }

    public void WriteLog(string message)
    {
        _log.Add(message);
        MessageGenerated?.Invoke(message);
    }
}
