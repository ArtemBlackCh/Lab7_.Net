namespace CityAlerts;

public class WeatherEventArgs : EventArgs
{
    public double WindSpeed { get; }
    public double Temperature { get; }
    public string Message { get; }
    public DateTime Time { get; }

    public WeatherEventArgs(double windSpeed, double temperature, string message, DateTime time)
    {
        WindSpeed = windSpeed;
        Temperature = temperature;
        Message = message;
        Time = time;
    }
}
