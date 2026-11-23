using CityAlerts;

namespace CityAlerts.Tests;

public class WeatherStationTests
{
    private static WeatherStation CreateStation(double initialTemperature = 15)
    {
        return new WeatherStation(new Megapolis("Київ", 2950000, 839, 10, 3), initialTemperature);
    }

    [Theory]
    [InlineData(26)]
    [InlineData(30)]
    [InlineData(50)]
    public void StormWarning_IsRaised_WhenWindAboveThreshold(double wind)
    {
        var station = CreateStation();
        int count = 0;
        station.StormWarning += (s, e) => count++;

        station.UpdateMeasurements(wind, 15);

        Assert.Equal(1, count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(20)]
    [InlineData(25)]
    public void StormWarning_IsNotRaised_WhenWindAtOrBelowThreshold(double wind)
    {
        var station = CreateStation();
        int count = 0;
        station.StormWarning += (s, e) => count++;

        station.UpdateMeasurements(wind, 15);

        Assert.Equal(0, count);
    }

    [Fact]
    public void StormWarning_IsBroadcast_ToAllThreeSubscribers()
    {
        var station = CreateStation();
        var emergency = new EmergencyService();
        var administration = new CityAdministration();
        var powerGrid = new PowerGrid();
        station.StormWarning += emergency.OnAlert;
        station.StormWarning += administration.OnAlert;
        station.StormWarning += powerGrid.OnAlert;

        station.UpdateMeasurements(30, 15);

        Assert.True(emergency.BrigadesReady);
        Assert.Equal(1, administration.SmsSentCount);
        Assert.True(powerGrid.EnhancedMode);
    }

    [Fact]
    public void StormWarning_IsNotDelivered_AfterUnsubscribe()
    {
        var station = CreateStation();
        var emergency = new EmergencyService();
        var powerGrid = new PowerGrid();
        station.StormWarning += emergency.OnAlert;
        station.StormWarning += powerGrid.OnAlert;
        station.StormWarning -= emergency.OnAlert;

        station.UpdateMeasurements(30, 15);

        Assert.False(emergency.BrigadesReady);
        Assert.True(powerGrid.EnhancedMode);
    }

    [Fact]
    public void StormWarning_EventArgs_ContainMeasurements()
    {
        var station = CreateStation();
        WeatherEventArgs? received = null;
        station.StormWarning += (s, e) => received = e;

        station.UpdateMeasurements(33, 12);

        Assert.NotNull(received);
        Assert.Equal(33, received!.WindSpeed);
        Assert.Equal(12, received.Temperature);
        Assert.False(string.IsNullOrEmpty(received.Message));
    }

    [Fact]
    public void TemperatureDrop_IsRaised_WhenTemperatureFallsByThreshold()
    {
        var station = CreateStation(15);
        var administration = new CityAdministration();
        var powerGrid = new PowerGrid();
        station.TemperatureDrop += administration.OnAlert;
        station.TemperatureDrop += powerGrid.OnAlert;

        station.UpdateMeasurements(5, 5);

        Assert.Equal(1, administration.SmsSentCount);
        Assert.True(powerGrid.EnhancedMode);
    }

    [Fact]
    public void TemperatureDrop_IsNotRaised_WhenTemperatureFallsSlightly()
    {
        var station = CreateStation(15);
        int count = 0;
        station.TemperatureDrop += (s, e) => count++;

        station.UpdateMeasurements(5, 10);

        Assert.Equal(0, count);
    }

    [Fact]
    public void StormCleared_IsRaised_WhenWindDropsAfterStorm()
    {
        var station = CreateStation();
        int count = 0;
        station.StormCleared += (s, e) => count++;

        station.UpdateMeasurements(30, 15);
        station.UpdateMeasurements(10, 15);

        Assert.Equal(1, count);
    }

    [Fact]
    public void StormCleared_IsNotRaised_WithoutPreviousStorm()
    {
        var station = CreateStation();
        int count = 0;
        station.StormCleared += (s, e) => count++;

        station.UpdateMeasurements(10, 15);

        Assert.Equal(0, count);
    }

    [Fact]
    public void MeasurementUpdated_IsRaised_OnEveryUpdate()
    {
        var station = CreateStation();
        int count = 0;
        station.MeasurementUpdated += (s, e) => count++;

        station.UpdateMeasurements(1, 15);
        station.UpdateMeasurements(2, 15);
        station.UpdateMeasurements(3, 15);

        Assert.Equal(3, count);
    }

    [Fact]
    public void UpdateMeasurements_StoresValues()
    {
        var station = CreateStation();

        station.UpdateMeasurements(7, -3);

        Assert.Equal(7, station.WindSpeed);
        Assert.Equal(-3, station.Temperature);
    }

    [Fact]
    public void UpdateMeasurements_Throws_WhenWindIsNegative()
    {
        var station = CreateStation();

        Assert.Throws<ArgumentException>(() => station.UpdateMeasurements(-1, 15));
    }
}
