using System;

public interface IWeatherService
{
    void GetCurrentWeather(
        float latitude,
        float longitude,
        Action<WeatherData> onSuccess,
        Action<string> onFailure);
}
