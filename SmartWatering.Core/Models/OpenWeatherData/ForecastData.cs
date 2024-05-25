namespace SmartWatering.Core.Models.OpenWeatherData;

public class ForecastData
{
    public string Cod { get; set; }

    public List<WeatherForecast> List { get; set; }

    public City City { get; set; }
}

public class WeatherForecast
{
    public Main Main { get; set; }

    public List<Weather> Weather { get; set; }

    public Clouds Clouds { get; set; }

    public Wind Wind { get; set; }

    public double Pop { get; set; }

    public string Dt_txt { get; set; }
}

public class Main
{
    public double Temp { get; set; }

    public double Feels_like { get; set; }

    public int Humidity { get; set; }
}

public class Weather
{
    public int Id { get; set; }

    public string Main { get; set; }

    public string Description { get; set; }

    public string Icon { get; set; }
}

public class Clouds
{
    public int All { get; set; }
}

public class Wind
{
    public double Speed { get; set; }

    public int Deg { get; set; }
}

public class City
{
    public string Name { get; set; }

    public Coord Coord { get; set; }

    public string Country { get; set; }

    public int Population { get; set; }

    public int Timezone { get; set; }
}

public class Coord
{
    public double Lat { get; set; }

    public double Lon { get; set; }
}
