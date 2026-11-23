using CityAlerts;

namespace CityAlerts.Tests;

public class HierarchyAndDelegateTests
{
    [Fact]
    public void Megapolis_IsCity()
    {
        City city = new Megapolis("Київ", 2950000, 839, 10, 3);

        Assert.IsType<Megapolis>(city);
        Assert.Equal("Київ", city.Name);
    }

    [Fact]
    public void Megapolis_GetInfo_ExtendsCityInfo()
    {
        var city = new City("Львів", 720000, 182);
        var megapolis = new Megapolis("Київ", 2950000, 839, 10, 3);

        Assert.Contains("Львів", city.GetInfo());
        Assert.Contains("Київ", megapolis.GetInfo());
        Assert.Contains("районів 10", megapolis.GetInfo());
        Assert.Contains("ліній метро 3", megapolis.GetInfo());
    }

    [Fact]
    public void Services_ImplementAllThreeInterfaces()
    {
        object[] services = { new EmergencyService(), new CityAdministration(), new PowerGrid() };

        foreach (var service in services)
        {
            Assert.IsAssignableFrom<IAlertSubscriber>(service);
            Assert.IsAssignableFrom<ILoggable>(service);
            Assert.IsAssignableFrom<IActionExecutor>(service);
        }
    }

    [Fact]
    public void EmergencyService_Execute_PreparesBrigades()
    {
        var service = new EmergencyService();
        string? message = null;
        service.MessageGenerated += m => message = m;

        service.Execute();

        Assert.True(service.BrigadesReady);
        Assert.NotNull(message);
        Assert.Single(service.Log);
    }

    [Fact]
    public void PowerGrid_Execute_SwitchesToEnhancedMode()
    {
        var service = new PowerGrid();

        service.Execute();

        Assert.True(service.EnhancedMode);
        Assert.Single(service.Log);
    }

    [Fact]
    public void ActionChain_IsMulticast_WithTwoMethods()
    {
        var administration = new CityAdministration();

        Assert.NotNull(administration.ActionChain);
        Assert.Equal(2, administration.ActionChain!.GetInvocationList().Length);
    }

    [Fact]
    public void ActionChain_Invocation_CallsAllMethodsInChain()
    {
        var administration = new CityAdministration();

        administration.Execute();

        Assert.Equal(1, administration.SmsSentCount);
        Assert.Equal(2, administration.Log.Count);
    }

    [Fact]
    public void ActionChain_RemovedMethod_IsNotCalled()
    {
        var administration = new CityAdministration();
        administration.ActionChain -= administration.SendSms;

        administration.Execute();

        Assert.Equal(0, administration.SmsSentCount);
        Assert.Single(administration.Log);
    }

    [Fact]
    public void ActionChain_AddedMethod_IsCalled()
    {
        var administration = new CityAdministration();
        var received = new List<string>();
        administration.ActionChain += received.Add;

        administration.Execute();

        Assert.Single(received);
        Assert.Equal(administration.ActionDescription, received[0]);
    }

    [Fact]
    public void WriteLog_AddsMessageAndRaisesEvent()
    {
        var administration = new CityAdministration();
        var messages = new List<string>();
        administration.MessageGenerated += messages.Add;

        administration.WriteLog("тест");

        Assert.Equal("тест", administration.Log[0]);
        Assert.Equal("тест", messages[0]);
    }
}
