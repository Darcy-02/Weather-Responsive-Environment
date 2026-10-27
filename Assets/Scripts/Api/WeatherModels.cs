using System;


[Serializable]
public class WeatherData
{
    public string name;
    public int id;
    public Coord coord;
    public Weather[] weather;
    public MainData main;
    public int visibility;
    public Wind wind;
    public Sys sys;
    public long dt;
    public int timezone;
}

[Serializable]
public class Coord
{
    public float lon;
    public float lat;
}

[Serializable]
public class Weather
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

[Serializable]
public class MainData
{
    public float temp;
    public float feels_like;
    public float temp_min;
    public float temp_max;
    public int pressure;
    public int humidity;
}

[Serializable]
public class Wind
{
    public float speed;
    public int deg;
    public float gust;
}

[Serializable]
public class Sys
{
    public string country;
    public long sunrise; 
    public long sunset;  


    public DateTime SunriseUtc => DateTimeOffset.FromUnixTimeSeconds(sunrise).UtcDateTime;
    public DateTime SunsetUtc => DateTimeOffset.FromUnixTimeSeconds(sunset).UtcDateTime;
}


[Serializable]
public class ApiError
{
    public string message;
}
