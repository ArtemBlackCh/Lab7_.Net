namespace CityAlerts;

public class WeatherStation
{
    public const double StormThreshold = 25;
    public const double DropThreshold = 10;

    private bool _stormActive;

    public event WeatherEventHandler? StormWarning;
    public event WeatherEventHandler? TemperatureDrop;
    public event WeatherEventHandler? StormCleared;
    public event WeatherEventHandler? MeasurementUpdated;

    public City City { get; }
    public double WindSpeed { get; private set; }
    public double Temperature { get; private set; }

    public WeatherStation(City city, double initialTemperature)
    {
        City = city;
        Temperature = initialTemperature;
    }

    public void UpdateMeasurements(double wind, double temp)
    {
        if (wind < 0)
        {
            throw new ArgumentException("Швидкість вітру не може бути від'ємною");
        }

        double previousTemperature = Temperature;
        WindSpeed = wind;
        Temperature = temp;

        MeasurementUpdated?.Invoke(this, CreateArgs($"Вимірювання: вітер {wind} м/с, температура {temp}°C"));

        if (wind > StormThreshold)
        {
            _stormActive = true;
            StormWarning?.Invoke(this, CreateArgs($"Штормове попередження! Швидкість вітру {wind} м/с"));
        }
        else if (_stormActive)
        {
            _stormActive = false;
            StormCleared?.Invoke(this, CreateArgs($"Шторм минув. Швидкість вітру {wind} м/с"));
        }

        if (previousTemperature - temp >= DropThreshold)
        {
            TemperatureDrop?.Invoke(this, CreateArgs($"Різке похолодання: {previousTemperature}°C → {temp}°C"));
        }
    }

    private WeatherEventArgs CreateArgs(string message)
    {
        return new WeatherEventArgs(WindSpeed, Temperature, message, DateTime.Now);
    }
}
