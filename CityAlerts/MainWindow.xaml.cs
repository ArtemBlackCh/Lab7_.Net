using System.Globalization;
using System.Windows;

namespace CityAlerts;

public partial class MainWindow : Window
{
    private WeatherStation station;
    private EmergencyService emergency;
    private CityAdministration administration;
    private PowerGrid powerGrid;

    public MainWindow()
    {
        InitializeComponent();

        station = new WeatherStation(new Megapolis("Київ", 2950000, 839, 10, 3), 15);
        emergency = new EmergencyService();
        administration = new CityAdministration();
        powerGrid = new PowerGrid();

        station.StormWarning += emergency.OnAlert;
        station.StormWarning += administration.OnAlert;
        station.StormWarning += powerGrid.OnAlert;

        station.TemperatureDrop += administration.OnAlert;
        station.TemperatureDrop += powerGrid.OnAlert;

        station.MeasurementUpdated += LogWeatherEvent;
        station.StormWarning += LogWeatherEvent;
        station.StormCleared += LogWeatherEvent;
        station.TemperatureDrop += LogWeatherEvent;

        emergency.MessageGenerated += LogMessage;
        administration.MessageGenerated += LogMessage;
        powerGrid.MessageGenerated += LogMessage;

        LogMessage(station.City.GetInfo());
    }

    private void btnApply_Click(object sender, RoutedEventArgs e)
    {
        if (TryReadValue(txtWind.Text, out double wind) && TryReadValue(txtTemperature.Text, out double temp))
        {
            try
            {
                station.UpdateMeasurements(wind, temp);
            }
            catch (Exception ex)
            {
                LogMessage($"ПОМИЛКА: {ex.Message}");
            }
        }
        else
        {
            LogMessage("ПОМИЛКА: Введіть коректні числові значення.");
        }
    }

    private bool TryReadValue(string text, out double value)
    {
        return double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private void LogWeatherEvent(object sender, WeatherEventArgs e)
    {
        LogMessage($"Метеостанція: {e.Message}");
    }

    private void LogMessage(string message)
    {
        logListBox.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        logListBox.ScrollIntoView(logListBox.Items[logListBox.Items.Count - 1]);
    }
}
