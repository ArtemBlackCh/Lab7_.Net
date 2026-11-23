namespace CityAlerts;

public class CityAdministration : IAlertSubscriber, ILoggable, IActionExecutor
{
    private readonly List<string> _log = new();

    public event Action<string>? MessageGenerated;

    public CityActionHandler? ActionChain { get; set; }

    public string Name => "Мерія";
    public int SmsSentCount { get; private set; }
    public IReadOnlyList<string> Log => _log;
    public string ActionDescription => "Розіслати SMS громадянам";

    public CityAdministration()
    {
        ActionChain += SendSms;
        ActionChain += RegisterInJournal;
    }

    public void OnAlert(object sender, WeatherEventArgs e)
    {
        ActionChain?.Invoke(e.Message);
    }

    public void SendSms(string message)
    {
        SmsSentCount++;
        WriteLog($"{Name}: {ActionDescription} - {message}");
    }

    public void RegisterInJournal(string message)
    {
        WriteLog($"{Name}: запис у журналі - {message}");
    }

    public void Execute()
    {
        ActionChain?.Invoke(ActionDescription);
    }

    public void WriteLog(string message)
    {
        _log.Add(message);
        MessageGenerated?.Invoke(message);
    }
}
