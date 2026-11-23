namespace CityAlerts;

public class EmergencyService : IAlertSubscriber, ILoggable, IActionExecutor
{
    private readonly List<string> _log = new();

    public event Action<string>? MessageGenerated;

    public string Name => "ДСНС";
    public bool BrigadesReady { get; private set; }
    public IReadOnlyList<string> Log => _log;
    public string ActionDescription => "Підготувати бригади";

    public void OnAlert(object sender, WeatherEventArgs e)
    {
        PrepareBrigades();
    }

    public void PrepareBrigades()
    {
        BrigadesReady = true;
        WriteLog($"{Name}: {ActionDescription}!");
    }

    public void Execute()
    {
        PrepareBrigades();
    }

    public void WriteLog(string message)
    {
        _log.Add(message);
        MessageGenerated?.Invoke(message);
    }
}
