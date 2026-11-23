namespace CityAlerts;

public class City
{
    public string Name { get; }
    public int Population { get; }
    public double Area { get; }

    public City(string name, int population, double area)
    {
        Name = name;
        Population = population;
        Area = area;
    }

    public virtual string GetInfo()
    {
        return $"Місто {Name}: населення {Population}, площа {Area} км²";
    }
}

public class Megapolis : City
{
    public int Districts { get; }
    public int SubwayLines { get; }

    public Megapolis(string name, int population, double area, int districts, int subwayLines)
        : base(name, population, area)
    {
        Districts = districts;
        SubwayLines = subwayLines;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + $", районів {Districts}, ліній метро {SubwayLines}";
    }
}
