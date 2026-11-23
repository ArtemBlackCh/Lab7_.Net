namespace CityAlerts;

public delegate void WeatherEventHandler(object sender, WeatherEventArgs e);

public delegate void CityActionHandler(string message);
